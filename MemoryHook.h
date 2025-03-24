#pragma once

struct PlayerPosition
{
    float x;
    float y;
    float z;
};

class MemoryHook
{
public:
    static bool Init();
    static PlayerPosition GetPlayerPosition();
};
