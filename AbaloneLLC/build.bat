@echo off

echo Preparing OneClick aplication..
echo.

SET FileToDelete="OneClick\Emgu.CV.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\Emgu.Util.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\FaceSDK.NET.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\Ionic.Zip.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\OpenTK.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\OpenTK.GLControl.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\RH.AssimpNet.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="OneClick\RH.ImageListView.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

echo.
echo Preparing PrintAhead_Paypal aplication..
echo.

SET FileToDelete="PrintAhead_Paypal\Emgu.CV.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete@ was removed!
)

SET FileToDelete="PrintAhead_Paypal\Emgu.Util.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="PrintAhead_Paypal\FaceSDK.NET.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="PrintAhead_Paypal\Ionic.Zip.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="PrintAhead_Paypal\OpenTK.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="PrintAhead_Paypal\OpenTK.GLControl.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="PrintAhead_Paypal\RH.AssimpNet.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

SET FileToDelete="PrintAhead_Paypal\RH.ImageListView.dll"
IF EXIST %FileToDelete% (
 del /F %FileToDelete%
 echo %FileToDelete% was removed!
)

echo.>"C:\Users\Kulikov\Documents\Visual Studio 2015\Projects\Laslo\AballoneLLC\AbaloneLLC\dblank.txt"

echo.
echo Ready for packing!