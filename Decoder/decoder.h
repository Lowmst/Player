#pragma once
#include "ffmpeg.h"

using PCMParameters = struct {
	int sample_rate;
	int bits_per_sample;
	int is_lossless;
};

using PCMPacket = struct {
	size_t size;
	char* data;
};

class Decoder {
public:
	explicit Decoder(const char* url);
	[[nodiscard]] PCMParameters Setup() const;
	PCMPacket Decode();

private:
	AVFormatContext* format_context_ = avformat_alloc_context();
	AVPacket* packet_ = av_packet_alloc();
	AVFrame* frame_ = av_frame_alloc();

	AVCodecContext* codec_context_;
	AVCodecParameters* codec_parameters_;
	const AVCodec* codec_;

	int audio_stream_index_ = 0;
	char* buffer_ = nullptr;

	int bits_per_sample_;
	int bytes_per_sample_;

	void Muxer(size_t size);

	int is_lossless_ = true;
	bool flushing_ = false;
};
