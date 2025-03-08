#include "dllexport.h"
#include "decoder.h"


Decoder* init(const char* url) {
	return new Decoder(url);
}

WAVEFORMATEX* setup(Decoder* decoder) {
	return decoder->Setup();
}

PCMPacket decode(Decoder* decoder) {
	return decoder->Decode();
}