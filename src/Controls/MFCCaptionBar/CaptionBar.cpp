#include "CaptionBar.h"
#include "framework.h"

CaptionBar::CaptionBar()
{
}

void CaptionBar::Show()
{
	static CMFCCaptionBar m_wndCaptionBar;

	if (!_isCreated)
	{
		BOOL result = m_wndCaptionBar.Create(WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS, AfxGetMainWnd(), 100000, -1, TRUE);
	}

	m_wndCaptionBar.SetText(this->Text, CMFCCaptionBar::ALIGN_LEFT);
	m_wndCaptionBar.ShowWindow(SW_SHOW);

	dynamic_cast<CFrameWnd*>(AfxGetMainWnd())->RecalcLayout(FALSE);

	_isCreated = true;
}

System::String^ CaptionBar::Text::get()
{
	return _text;
}

void CaptionBar::Text::set(System::String^ value)
{
	_text = value;
}
