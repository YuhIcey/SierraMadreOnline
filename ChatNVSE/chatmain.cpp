#include "nvse/PluginAPI.h"
#include "nvse/CommandTable.h"
#include <Windows.h>
#include <string>
#include <vector>
#include <sstream>
#include <thread>
#include <mutex>
#include <iostream>
#include <functional>

// ---------------------------------------------------------------------
// Forward declarations for NVSE functions
// ---------------------------------------------------------------------
extern "C" {
    void Console_Print(const char* fmt, ...);
    void PrintHUDMessage(const char* msg);
}

// ---------------------------------------------------------------------
// Simple macro to replace _MESSAGE (using Console_Print)
#define _MESSAGE(...) Console_Print(__VA_ARGS__)

// ---------------------------------------------------------------------
// Dummy WebSocket Client
// Replace this with a real library if needed later.
namespace WebSocketClient
{
    // Callback for received messages
    static std::function<void(const std::string&)> OnMessage;

    // Initialize a connection (dummy implementation)
    void Init(const std::string& url)
    {
        std::cout << "[WebSocketClient] Connecting to: " << url << std::endl;
    }

    // Send a message (dummy implementation that echoes the message back)
    void Send(const std::string& msg)
    {
        std::cout << "[WebSocketClient] Sending: " << msg << std::endl;
        if (OnMessage)
        {
            // Echo the message back as if the server responded
            OnMessage("{\"type\":\"chat\",\"mode\":\"global\",\"message\":\"Echo: " + msg + "\"}");
        }
    }
}

// ---------------------------------------------------------------------
// Global variables for the plugin
// ---------------------------------------------------------------------
PluginHandle g_pluginHandle = kPluginHandle_Invalid;
bool running = true;
static std::vector<std::string> ChatQueue;
static std::mutex ChatMutex;

// ---------------------------------------------------------------------
// Function declarations
// ---------------------------------------------------------------------
void PollInput();
void NetworkThread();
void SendChatMessage(const std::string& message, bool isGlobal);
void OnMessageReceived(const std::string& msg);
void RenderChat();

// ---------------------------------------------------------------------
// NVSE Plugin Entry Points
// ---------------------------------------------------------------------
extern "C"
{
    bool NVSEPlugin_Query(const NVSEInterface* nvse, PluginInfo* info)
    {
        info->infoVersion = PluginInfo::kInfoVersion;
        info->name = "MultiplayerChat";
        info->version = 1;
        g_pluginHandle = nvse->GetPluginHandle();
        // Don't load in the editor
        return !nvse->isEditor;
    }

    bool NVSEPlugin_Load(const NVSEInterface* nvse)
    {
        _MESSAGE("🚀 MultiplayerChat Loaded");

        std::thread inputThread(PollInput);
        inputThread.detach();

        std::thread netThread(NetworkThread);
        netThread.detach();

        return true;
    }
}

// ---------------------------------------------------------------------
// PollInput: Periodically checks for ENTER key to read chat input
// ---------------------------------------------------------------------
void PollInput()
{
    while (running)
    {
        if (GetAsyncKeyState(VK_RETURN) & 1)
        {
            char buffer[256] = { 0 };
            Console_Print("Enter chat: ");
            std::cin.getline(buffer, sizeof(buffer));
            std::string msg(buffer);
            if (!msg.empty())
            {
                // If the message starts with "//", treat it as a global message.
                bool global = (msg.rfind("//", 0) == 0);
                SendChatMessage(global ? msg.substr(2) : msg, global);
            }
        }
        Sleep(50);
    }
}

// ---------------------------------------------------------------------
// SendChatMessage: Builds a JSON message and sends it via the dummy WebSocket client
// ---------------------------------------------------------------------
void SendChatMessage(const std::string& message, bool isGlobal)
{
    std::ostringstream json;
    json << "{"
         << "\"type\":\"chat\","
         << "\"mode\":\"" << (isGlobal ? "global" : "local") << "\","
         << "\"message\":\"" << message << "\""
         << "}";
    WebSocketClient::Send(json.str());
    Console_Print("You: %s", message.c_str());
}

// ---------------------------------------------------------------------
// OnMessageReceived: Adds received messages to a thread-safe queue
// ---------------------------------------------------------------------
void OnMessageReceived(const std::string& msg)
{
    std::lock_guard<std::mutex> lock(ChatMutex);
    ChatQueue.push_back(msg);
}

// ---------------------------------------------------------------------
// RenderChat: Displays queued chat messages on the HUD
// ---------------------------------------------------------------------
void RenderChat()
{
    std::lock_guard<std::mutex> lock(ChatMutex);
    for (auto& line : ChatQueue)
    {
        PrintHUDMessage(line.c_str());
    }
    ChatQueue.clear();
}

// ---------------------------------------------------------------------
// NetworkThread: Initializes the dummy WebSocket client and periodically renders chat
// ---------------------------------------------------------------------
void NetworkThread()
{
    WebSocketClient::OnMessage = [](const std::string& msg)
    {
        OnMessageReceived(msg);
    };

    WebSocketClient::Init("ws://localhost:7778");

    while (running)
    {
        RenderChat();
        Sleep(100);
    }
}
