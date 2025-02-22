#pragma once
#include "decoder.h"
#define DLL extern "C" __declspec(dllexport)

DLL Decoder* Init(const char* url);
DLL PCMParameters Setup(Decoder* decoder);
DLL PCMPacket Decode(Decoder* decoder);