import std;
#include "coreaudio.h"

int main()
{
	std::wcout.imbue(std::locale("zh_CN"));

	HRESULT hr = CoInitialize(nullptr);

	IMMDeviceEnumerator* device_enumerator;
	IMMDeviceCollection* device_collection;
	IMMDevice* device;
	IAudioClient* audio_client;

	PROPVARIANT prop_variant;
	IPropertyStore* property_store;
	PropVariantInit(&prop_variant);

	hr = CoCreateInstance(
		__uuidof(MMDeviceEnumerator),
		nullptr,
		CLSCTX_ALL,
		__uuidof(IMMDeviceEnumerator),
		reinterpret_cast<LPVOID*>(&device_enumerator)
	);

	hr = device_enumerator->EnumAudioEndpoints(
		eRender,
		DEVICE_STATE_ACTIVE,
		&device_collection
	);

	unsigned int nb_devices;
	hr = device_collection->GetCount(&nb_devices);
	std::println("{}", nb_devices);

	for (int i = 0; i < nb_devices; ++i)
	{
		LPWSTR* device_id = nullptr;

		hr = device_collection->Item(i, &device);

		hr = device->GetId(device_id);

		hr = device->OpenPropertyStore(STGM_READ, &property_store);

		hr = property_store->GetValue(PKEY_Device_FriendlyName, &prop_variant);

		std::wcout << prop_variant.pwszVal << std::endl;
		
	}

	// default device

	hr = device_enumerator->GetDefaultAudioEndpoint(eRender, eMultimedia, &device);
	hr = device->OpenPropertyStore(STGM_READ, &property_store);
	hr = property_store->GetValue(PKEY_Device_FriendlyName, &prop_variant);
	std::wcout << prop_variant.pwszVal << std::endl;

	hr = device->Activate(
		__uuidof(IAudioClient),
		CLSCTX_ALL,
		nullptr,
		reinterpret_cast<void**>(&audio_client)
	);

	UINT32 size;
	

	WAVEFORMATEX wave_format
	{
		WAVE_FORMAT_PCM,
		2,
		44100,
		176400,
		4,
		16,
		0
	};

	hr = audio_client->Initialize(
		AUDCLNT_SHAREMODE_EXCLUSIVE,
		0,
		1000000, // 100 ms
		0,
		&wave_format,
		nullptr
	);
	//if (FAILED(hr)) exit(hr);
	hr = audio_client->GetBufferSize(&size);
	
	
	std::println("{}", AUDCLNT_E_UNSUPPORTED_FORMAT);
	
	return 0;
}