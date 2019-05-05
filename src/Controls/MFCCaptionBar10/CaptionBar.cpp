#include "CaptionBar.h"
#include <windows.h>
#include <strsafe.h>
#include <msclr\marshal.h>

using namespace System;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

CaptionBar::CaptionBar()
{
	del = gcnew GetMsgProcDelegate(this, &CaptionBar::GetMsgProc);
}

LRESULT CALLBACK CaptionBar::GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode < 0) // do not process message 
		return CallNextHookEx(hook, nCode, wParam, lParam);

	CWPSTRUCT cwp = *(CWPSTRUCT*)lParam;
	if (cwp.message == WM_COMMAND)
	{
		if (cwp.wParam == ID_TOOLS_OPTIONS)
		{
			OnButtonClicked(this, gcnew System::EventArgs());
		}
	}

	return CallNextHookEx(hook, nCode, wParam, lParam);
}

static GCHandle gch;
static CMFCCaptionBar m_wndCaptionBar;

void CaptionBar::Create()
{
	//if (!m_wndCaptionBar.Create(WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS, this, ID_VIEW_CAPTION_BAR, -1, TRUE))
	BOOL result = m_wndCaptionBar.Create(WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS, AfxGetMainWnd(), 100000, -1, TRUE);

	gch = GCHandle::Alloc(del);
	IntPtr ip = Marshal::GetFunctionPointerForDelegate(del);
	HOOKPROC hp = static_cast<HOOKPROC>(ip.ToPointer());
	hook = SetWindowsHookEx(WH_CALLWNDPROC, hp, (HINSTANCE)NULL, GetCurrentThreadId());

	_isCreated = true;
}

void CaptionBar::Update()
{
	if (!_isCreated)
	{
		Create();
	}

	//Create the button
	if (DisplayButton)
	{
		marshal_context context;
		LPCTSTR cstrT = context.marshal_as<const TCHAR*>(this->ButtonText);
		LPCTSTR cstrTT = context.marshal_as<const TCHAR*>(this->ButtonToolTip);

		m_wndCaptionBar.SetButton(cstrT, ID_TOOLS_OPTIONS, CMFCCaptionBar::ALIGN_LEFT, FALSE);
		m_wndCaptionBar.SetButtonToolTip(cstrTT);
	}

	if (this->Image != nullptr)
	{
		HBITMAP hbitMap = (HBITMAP)this->Image->GetHbitmap().ToInt32();
		m_wndCaptionBar.SetBitmap(hbitMap, RGB(255, 255, 255), FALSE, CMFCCaptionBar::ALIGN_LEFT);
	}

	m_wndCaptionBar.SetText(this->Text, CMFCCaptionBar::ALIGN_LEFT);
}

void CaptionBar::Show()
{
	if (!_isCreated)
	{
		Create();
	}

	m_wndCaptionBar.ShowWindow(SW_SHOW);

	Update();
}

Bitmap^ CaptionBar::Image::get()
{
	return _image;
}

void CaptionBar::Image::set(Bitmap^ value)
{
	_image = value;
}

System::String^ CaptionBar::Text::get()
{
	return _messageText;
}

void CaptionBar::Text::set(System::String^ value)
{
	_messageText = value;
	Update();
}

System::String^ CaptionBar::ButtonText::get()
{
	return _buttonText;
}

void CaptionBar::ButtonText::set(System::String^ value)
{
	_buttonText = value;
}

System::String^ CaptionBar::ButtonToolTip::get()
{
	return _buttonToolTip;
}

void CaptionBar::ButtonToolTip::set(System::String^ value)
{
	_buttonToolTip = value;
}

System::Boolean^ CaptionBar::DisplayButton::get()
{
	return _displayButton;
}

void CaptionBar::DisplayButton::set(System::Boolean^ value)
{
	_displayButton = value;
}