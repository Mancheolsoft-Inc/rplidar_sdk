using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class RPlidarDriverWrapper : MonoBehaviour
{
    // C++ DLL 함수 선언
    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateDriverInstance(uint drivertype);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisposeDriverInstance(IntPtr driver);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ConnectDriver(IntPtr driver, string path, uint portOrBaud, uint flag);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisconnectDriver(IntPtr driver);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsDriverConnected(IntPtr driver);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ResetDriver(IntPtr driver, uint timeout);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartScanDriver(IntPtr driver, bool force, bool useTypicalScan);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopScanDriver(IntPtr driver);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetDeviceInfoDriver(IntPtr driver, out DeviceInfo info);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetMotorPWMDriver(IntPtr driver, ushort pwm);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StartMotorDriver(IntPtr driver);

    [DllImport("rplidar_driver", CallingConvention = CallingConvention.Cdecl)]
    public static extern int StopMotorDriver(IntPtr driver);

    // LIDAR 장치 정보 구조체
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] serialNumber;
        public byte model;
        public byte firmwareVersionMinor;
        public byte firmwareVersionMajor;
        public byte hardwareVersion;
    }

    // Unity에서 사용할 MonoBehaviour 메서드
    private IntPtr driver;

    void Start()
    {
        // 드라이버 생성
        driver = CreateDriverInstance(0); // 0: 기본 드라이버 타입

        // LIDAR 연결
        int result = ConnectDriver(driver, "COM3", 115200, 0);
        if (result == 0) // SL_RESULT_OK
        {
            Debug.Log("LIDAR 연결 성공");

            // 장치 정보 가져오기
            DeviceInfo info;
            if (GetDeviceInfoDriver(driver, out info) == 0)
            {
                Debug.Log($"LIDAR 모델: {info.model}, 펌웨어 버전: {info.firmwareVersionMajor}.{info.firmwareVersionMinor}");
            }

            // 스캔 시작
            StartScanDriver(driver, false, true);
        }
        else
        {
            Debug.LogError("LIDAR 연결 실패");
        }
    }

    void OnDestroy()
    {
        // 스캔 중지 및 드라이버 삭제
        if (driver != IntPtr.Zero)
        {
            StopScanDriver(driver);
            DisconnectDriver(driver);
            DisposeDriverInstance(driver);
            driver = IntPtr.Zero;
        }
    }
}
