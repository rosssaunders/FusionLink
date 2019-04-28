#pragma once

public ref class CaptionBar
{
private:
	System::String^ _text;
	System::Boolean^ _isCreated;

public:
	CaptionBar();

	void Show();

	property System::String^ Text
	{
		System::String^ get();
		void set(System::String^ value);
	}
};

