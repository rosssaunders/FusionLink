#ifndef _SafeRun_Version_H_
#define _SafeRun_Version_H_

#define SafeRun_VERSION 				1, 0, 0, 0
#define SafeRun_VERSION_STR			"1.0.0.0\0"
#ifdef WIN32
	#define SafeRun_TOOLKIT_DESCRIPTION	"SafeRun (x86)"
#else
	#define SafeRun_TOOLKIT_DESCRIPTION	"SafeRun (x64)"
#endif

#endif // _SafeRun_Version_H_
