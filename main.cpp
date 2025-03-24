#include "MemoryHook.h"
#include <iostream>
#include <thread>

int main()
{
    if (!MemoryHook::Init())
        return 1;

    while (true)
    {
        PlayerPosition pos = MemoryHook::GetPlayerPosition();
        std::cout << "📍 Pos: X=" << pos.x << " Y=" << pos.y << " Z=" << pos.z << "\n";
        std::this_thread::sleep_for(std::chrono::milliseconds(500));
    }

    return 0;
}
