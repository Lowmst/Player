#include "dllexport.h"
#include "decoder.h"

	
Decoder* Init(const char* url) {
	return new Decoder(url);
}

PCMParameters Setup(Decoder* decoder) {
	return decoder->Setup();
}

PCMPacket Decode(Decoder* decoder) {
	return decoder->Decode();
}