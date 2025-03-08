import std;
#include "decoder.h"



Decoder::Decoder(const char* url)
{
	av_log_set_level(AV_LOG_QUIET);

	avformat_open_input(&format_context_, url, nullptr, nullptr);
	avformat_find_stream_info(format_context_, nullptr);

	for (int i = 0; i < format_context_->nb_streams; i++)
	{
		if (format_context_->streams[i]->codecpar->codec_type == AVMEDIA_TYPE_AUDIO)
		{
			audio_stream_index_ = i;
		}
	}

	codec_parameters_ = format_context_->streams[audio_stream_index_]->codecpar;
	codec_ = avcodec_find_decoder(codec_parameters_->codec_id);
	codec_context_ = avcodec_alloc_context3(codec_);

	avcodec_parameters_to_context(codec_context_, codec_parameters_);
	avcodec_open2(codec_context_, codec_, nullptr);

	sample_rate_ = codec_parameters_->sample_rate;
	valid_bits_per_sample_ =
		codec_parameters_->bits_per_raw_sample ? codec_parameters_->bits_per_raw_sample : codec_parameters_->bits_per_coded_sample;
	bytes_per_sample_ = std::ceil(static_cast<float>(valid_bits_per_sample_) / 8);

	// decode first frame to access encode info
	// process with float-point sample
	while (av_read_frame(format_context_, packet_) == 0)
	{
		if (packet_->stream_index == audio_stream_index_)
		{
			avcodec_send_packet(codec_context_, packet_);
			if (avcodec_receive_frame(codec_context_, frame_) == 0)
			{
				if (frame_->format == AV_SAMPLE_FMT_FLT || frame_->format == AV_SAMPLE_FMT_FLTP)
				{
					is_ieee_float_ = true;
					valid_bits_per_sample_ = 32;
					bytes_per_sample_ = 4;
				}

				// no support for double float-point sample
				//if (frame_->format == AV_SAMPLE_FMT_DBL || frame_->format == AV_SAMPLE_FMT_DBLP)
				//{
				//	is_ieee_float_ = true;
				//	valid_bits_per_sample_ = 64;
				//	bytes_per_sample_ = 8;
				//}
				break;
			}
		}

	}
	av_frame_unref(frame_);
	av_packet_unref(packet_);
	avformat_close_input(&format_context_);
	avcodec_free_context(&codec_context_);

	// open url again
	avformat_open_input(&format_context_, url, nullptr, nullptr);
	avformat_find_stream_info(format_context_, nullptr);

	codec_parameters_ = format_context_->streams[audio_stream_index_]->codecpar;
	codec_ = avcodec_find_decoder(codec_parameters_->codec_id);
	codec_context_ = avcodec_alloc_context3(codec_);

	avcodec_parameters_to_context(codec_context_, codec_parameters_);
	avcodec_open2(codec_context_, codec_, nullptr);
}

WAVEFORMATEX* Decoder::Setup() const
{
	return reinterpret_cast<WAVEFORMATEX*>(
		new WAVEFORMATEXTENSIBLE
		{
			WAVEFORMATEX
			{
				WAVE_FORMAT_EXTENSIBLE,
				2,
				static_cast<unsigned long>(sample_rate_),
				static_cast<unsigned long>(sample_rate_ * bytes_per_sample_ * 2),
				static_cast<unsigned short>(bytes_per_sample_ * 2),
				static_cast<unsigned short>(bytes_per_sample_ * 8),
				22
			},
			static_cast<unsigned short>(valid_bits_per_sample_),
			SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT,
			is_ieee_float_ ? KSDATAFORMAT_SUBTYPE_IEEE_FLOAT : KSDATAFORMAT_SUBTYPE_PCM
		});
}

PCMPacket Decoder::Decode()
{
	if (avcodec_receive_frame(codec_context_, frame_) == 0)
	{
		const size_t size = 2ll * frame_->nb_samples * bytes_per_sample_;
		Muxer(size);
		return PCMPacket{ size, buffer_ };
	}

	av_packet_unref(packet_);

	while (av_read_frame(format_context_, packet_) == 0)
	{
		if (packet_->stream_index == audio_stream_index_)
		{
			avcodec_send_packet(codec_context_, packet_);
			if (avcodec_receive_frame(codec_context_, frame_) == 0)
			{
				const size_t size = 2ll * frame_->nb_samples * bytes_per_sample_;
				Muxer(size);
				return PCMPacket{ size, buffer_ };
			}
		}
	}

	return PCMPacket{ 0, nullptr };
}

void Decoder::Muxer(const size_t size)
{
	delete[] buffer_;
	buffer_ = new char[size];

	const auto format = static_cast<AVSampleFormat>(frame_->format);

	const int padding = av_get_bytes_per_sample(format) - bytes_per_sample_;

	if (av_sample_fmt_is_planar(format))
	{
		const size_t raw_size = 2ll * frame_->nb_samples * av_get_bytes_per_sample(format);
		const auto packed_samples = new char[raw_size];

		for (int i = 0; i < raw_size / 2; i += av_get_bytes_per_sample(format))
		{
			for (int j = 0; j < av_get_bytes_per_sample(format); j++)
			{
				packed_samples[2ll * i + j] = static_cast<char>(frame_->data[0][i + j]);
				packed_samples[2ll * i + j + av_get_bytes_per_sample(format)] = static_cast<char>(frame_->data[1][i + j]);
			}
		}

		if (padding == 0)
		{
			std::memcpy(buffer_, packed_samples, size);
		}
		else
		{
			for (int i = 0; i < size; i++)
			{
				buffer_[i] = packed_samples[i / bytes_per_sample_ + padding + i];
			}
		}
		delete[] packed_samples;
	}
	else
	{
		if (padding == 0)
		{
			std::memcpy(buffer_, frame_->data[0], size);
		}
		else
		{
			for (int i = 0; i < size; i++)
			{
				buffer_[i] = reinterpret_cast<char*>(frame_->data[0])[i / bytes_per_sample_ + padding + i];
			}
		}
	}

	av_frame_unref(frame_);
}