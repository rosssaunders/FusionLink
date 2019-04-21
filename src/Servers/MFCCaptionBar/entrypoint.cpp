#include <afxwin.h>
#include <afxdllx.h>

#ifdef __cplusplus_cli
#	error No CLR for this file
#endif

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//VC8 modification of the standard
static AFX_EXTENSION_MODULE sUserDll = {false,NULL, NULL, NULL, NULL };

extern "C" int APIENTRY
DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	// Remove this if you use lpReserved
	UNREFERENCED_PARAMETER(lpReserved);

	if (dwReason == DLL_PROCESS_ATTACH)
	{
		TRACE0("Projet_Utilisateur.DLL Initializing!\n");
		
		// Extension DLL one-time initialization
		if (!AfxInitExtensionModule(sUserDll, hInstance))
			return 0;
		// Insert this DLL into the resource chain
		// NOTE: If this Extension DLL is being implicitly linked to by
		//  an MFC Regular DLL (such as an ActiveX Control)
		//  instead of an MFC application, then you will want to
		//  remove this line from DllMain and put it in a separate
		//  function exported from this Extension DLL.  The Regular DLL
		//  that uses this Extension DLL should then explicitly call that
		//  function to initialize this Extension DLL.  Otherwise,
		//  the CDynLinkLibrary object will not be attached to the
		//  Regular DLL's resource chain, and serious problems will
		//  result.

		new CDynLinkLibrary(sUserDll);
	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{
		TRACE0("Projet_Utilisateur.DLL Terminating!\n");
		// Terminate the library before destructors are called
		AfxTermExtensionModule(sUserDll);
	}
	return 1;   // ok
}
