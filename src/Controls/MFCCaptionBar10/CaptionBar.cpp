#include "CaptionBar.h"
#include <windows.h>
#include <strsafe.h>
#include <msclr\marshal.h>

using namespace System;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

static CMFCCaptionBar m_wndCaptionBar;

void RxdSolutions::FusionLink::CaptionBar::Create()
{
	BOOL result = m_wndCaptionBar.Create(WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS, AfxGetMainWnd(), 100000, -1, TRUE);

	_isCreated = true;
}

void RxdSolutions::FusionLink::CaptionBar::Update()
{
	if (!_isCreated)
	{
		Create();
	}

	m_wndCaptionBar.SetText(this->Text, CMFCCaptionBar::ALIGN_LEFT);
}

void RxdSolutions::FusionLink::CaptionBar::Show()
{
	if (!_isCreated)
	{
		Create();
	}

	if (this->Image != nullptr)
	{
		HBITMAP hbitMap = (HBITMAP)this->Image->GetHbitmap().ToInt32();
		m_wndCaptionBar.SetBitmap(hbitMap, RGB(255, 255, 255), FALSE, CMFCCaptionBar::ALIGN_LEFT);
	}

	m_wndCaptionBar.ShowWindow(SW_SHOW);

	Update();
}

Bitmap^ RxdSolutions::FusionLink::CaptionBar::Image::get()
{
	return _image;
}

void RxdSolutions::FusionLink::CaptionBar::Image::set(Bitmap^ value)
{
	_image = value;
}

System::String^ RxdSolutions::FusionLink::CaptionBar::Text::get()
{
	return _messageText;
}

void RxdSolutions::FusionLink::CaptionBar::Text::set(System::String^ value)
{
	_messageText = value;

	if (_isCreated)
	{
		m_wndCaptionBar.SetText(this->Text, CMFCCaptionBar::ALIGN_LEFT);
	}
}
