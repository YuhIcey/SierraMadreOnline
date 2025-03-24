#include "MemoryHook.h"
#include <Windows.h>
#include <TlHelp32.h>
#include <iostream>

static DWORD GetFalloutNV_PID()
{
    DWORD pid = 0;
    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    PROCESSENTRY32 entry = { sizeof(PROCESSENTRY32) };

    if (Process32First(snapshot, &entry)) {
        do {
            if (_wcsicmp(entry.szExeFile, L"FalloutNV.exe") == 0) {
                pid = entry.th32ProcessID;
                break;
            }
        } while (Process32Next(snapshot, &entry));
    }

    CloseHandle(snapshot);
    return pid;
}

static HANDLE hProcess = nullptr;

// Replace with real offset once verified
static uintptr_t BASE_ADDRESS = 0x000000; // needs NV offset for player object

bool MemoryHook::Init()
{
    DWORD pid = GetFalloutNV_PID();
    if (pid == 0)
    {
        std::cerr << "❌ FalloutNV.exe not found.\n";
        return false;
    }

    hProcess = OpenProcess(PROCESS_VM_READ, FALSE, pid);
    if (!hProcess)
    {
        std::cerr << "❌ Failed to open process for FalloutNV.\n";
        return false;
    }

    std::cout << "✅ Connected to FalloutNV (PID: " << pid << ")\n";
    return true;
}

PlayerPosition MemoryHook::GetPlayerPosition()
{
    PlayerPosition pos = { 0 };

    // Example: Replace with real base + offset
    uintptr_t playerBase = BASE_ADDRESS + 0x00F4; // Example offset
    ReadProcessMemory(hProcess, (LPCVOID)playerBase, &pos, sizeof(pos), nullptr);

    return pos;
}
