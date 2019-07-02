#pragma once
#include "framework.h"

using namespace System;
using namespace System::Drawing;

namespace RxdSolutions
{
namespace FusionLink
{
	public ref class CaptionBar
	{
	private:
		System::Boolean^ _isCreated;

		System::String^ _messageText;
		Bitmap^ _image;

		void Create();
		void Update();

	public:

		void Show();

		property Bitmap^ Image
		{
			Bitmap^ get();
			void set(Bitmap^ value);
		}

		property System::String^ Text
		{
			System::String^ get();
			void set(System::String^ value);
		}

	protected:

	};
}
}


