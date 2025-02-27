# Player
Decode: FFmpeg

Playback: NAudio

It works on WASAPI Exclusive mode ~~, supporting lossless audio only~~.

Notice: Lossy audio is decoded into 32-bit floating point samples by FFmpeg now. Make sure device supporting.

**NAudio 在 WASAPI 独占模式下的缓冲区策略存在巨大缺陷, 体现在暂停后的继续播放会产生明显间断或杂音, 除非使用 Stop 方法. 在直接使用 Windows API 替代 NAudio 之前, 此仓库归档.**