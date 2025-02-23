#pragma once
#include "decoder.h"
#define DLL extern "C" __declspec(dllexport)

DLL Decoder* init(const char* url);
DLL PCMParameters setup(Decoder* decoder);
DLL PCMPacket decode(Decoder* decoder);