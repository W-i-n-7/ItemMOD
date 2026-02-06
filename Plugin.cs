global using BepInEx;
global using BepInEx.IL2CPP;
global using HarmonyLib;
global using UnityEngine;
global using System;
global using System.IO;
global using UnhollowerRuntimeLib;
using System.Linq;
using TMPro;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SteamworksNative;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Math = System.Math;
using UnityEngine.EventSystems;
using BepInEx.Logging;
using BepInEx.IL2CPP.Utils.Collections;
using UnityEngine.UI;

namespace itemMod
{
    [BepInPlugin("htMDT64UsYVG2LkapJ7wcz", "itemMod", "10.1")]
    public class Plugin : BasePlugin
    {
        public const string ver = "10c";
        public const int apiver = 15;

        public static ulong clientId = 0;

        public static string lobbyIDCheckURL = "";
        public static string DiscordURL = "";
        public static string WebsiteURL = "";
        public static string UpdateURL = "";

        // Program

        internal static new ManualLogSource Logger;
        public static string pluginsPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static readonly HttpClient httpClient = new HttpClient();
        public static string crabgamePath = pluginsPath.Replace("\\BepInEx\\plugins", "");
        public static string itemmodPath = $"{pluginsPath}\\ItemMod";
        
        // Internal values
        
        public static bool gameStarted = false;
        public static int itemToGive = -1;
        public static bool crabMode = false;
        public static bool crabModeAllowed = true;
        public static List<Il2CppSystem.Collections.Generic.Dictionary<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique>.Entry> playersList;
        public static List<Il2CppSystem.Collections.Generic.Dictionary<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique>.Entry> playersList2;
        public static Dictionary<ulong, int> playerDMG = new Dictionary<ulong, int>();
        public static TextMeshProUGUI spectatorsText = null;
        public static Dictionary<ulong, ulong> spectatorsList = new Dictionary<ulong, ulong>();
        public static List<ulong> spectatingME = new List<ulong>();
        public static float totalReloadTime = 0f;
        public static float currentReloadTime = 0f;

        // config

        public static bool dontReplaceItem = true;
        public static bool profanityBypass = false;
        public static bool fullprofanityBypass = true;
        public static bool jumpboostdisabled = false;
        public static float jumpboostamount = 20;
        public static string webhook = "";
        public static bool crabModeIsOn = true;
        public static bool show_ammo = true;
        public static bool dcplayerlisthaslinks = false;
        public static bool warn100dmg = false;
        public static bool skipIntro = true;
        public static bool show_gameid_chat = true;
        public static bool show_moregameid_chat = true;
        public static bool showBoxRates = false;
        public static bool hideAbout = true;
        public static bool IdToWebhook = true;
        public static bool IdToNotepad = true;
        public static bool playerlisttodcauto = false;
        public static bool unbanAkDual = true;
        public static bool autoCopyOnId = false;
        public static bool showSpectatorCount = false;
        public static bool spectatorCountBelowCrosshair = false;
        public static bool spectatorsInTAB = false;
        public static bool clearView = false;
        public static bool localdeathmessages = false;
        public static bool hostdeathmessages = false;
        public static bool localjoinleavemessages = false;
        public static bool hostjoinleavemessages = false;
        public static bool steamcmduseoverlay = false;

        // config (keybinds)

        public static int key_pizza = 112;
        public static int key_milk = 109;
        public static int key_grenade = 111;
        public static int key_jumpboost = 98;
        public static int key_ready = 107;
        public static int key_spin = 108;
        public static int key_kill = 290;
        public static int key_checktag = 117;
        public static int key_crab = 106;

        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Main>();

            Harmony.CreateAndPatchAll(typeof(Plugin));

            Logger = base.Log;

            Logger.LogInfo($"ItemMOD v{ver} Loaded.");
            Logger.LogInfo($"Made by Win_7");

            if (!Directory.Exists(itemmodPath)) {
                Directory.CreateDirectory(itemmodPath);
            }

            //File.WriteAllText($"{itemmodPath}\\log.txt", "");

            File.WriteAllText($"{itemmodPath}\\keys_for_keybinds.txt",
@"Backspace: 8
Delete: 127
Tab: 9
Clear: 12
Return: 13
Pause: 19
Escape: 27
Space: 32
Exclaim: 33
DoubleQuote: 34
Hash: 35
Dollar: 36
Percent: 37
Ampersand: 38
LeftParen: 40
RightParen: 41
Asterisk: 42
Plus: 43
Comma: 44
Minus: 45
Period: 46
Slash: 47
Colon: 58
Semicolon: 59
Less: 60
Equals: 61
Greater: 62
Question: 63
At: 64
LeftBracket: 91
Backslash: 92
RightBracket: 93
Caret: 94
Underscore: 95
BackQuote: 96
A: 97
B: 98
C: 99
D: 100
E: 101
F: 102
G: 103
H: 104
I: 105
J: 106
K: 107
L: 108
M: 109
N: 110
O: 111
P: 112
Q: 113
R: 114
S: 115
T: 116
U: 117
V: 118
W: 119
X: 120
Y: 121
Z: 122
Alpha0: 48
Alpha1: 49
Alpha2: 50
Alpha3: 51
Alpha4: 52
Alpha5: 53
Alpha6: 54
Alpha7: 55
Alpha8: 56
Alpha9: 57
LeftCurlyBracket: 123
Pipe: 124
RightCurlyBracket: 125
Tilde: 126
Numlock: 300
CapsLock: 301
ScrollLock: 302
RightShift: 303
LeftShift: 304
RightControl: 305
LeftControl: 306
RightAlt: 307
LeftAlt: 308
LeftCommand: 310
RightCommand: 309
LeftWindows: 311
RightWindows: 312
AltGr: 313
Help: 315
Print: 316
SysReq: 317
Break: 318
Menu: 319
Mouse0: 323
Mouse1: 324
Mouse2: 325
Mouse3: 326
Mouse4: 327
Mouse5: 328
Mouse6: 329
Keypad0: 256
Keypad1: 257
Keypad2: 258
Keypad3: 259
Keypad4: 260
Keypad5: 261
Keypad6: 262
Keypad7: 263
Keypad8: 264
Keypad9: 265
KeypadPeriod: 266
KeypadDivide: 267
KeypadMultiply: 268
KeypadMinus: 269
KeypadPlus: 270
KeypadEnter: 271
KeypadEquals: 272
UpArrow: 273
DownArrow: 274
RightArrow: 275
LeftArrow: 276
F1: 282
F2: 283
F3: 284
F4: 285
F5: 286
F6: 287
F7: 288
F8: 289
F9: 290
F10: 291
F11: 292
F12: 293
Insert: 277
Home: 278
End: 279
PageUp: 280
PageDown: 281
"
);

            if (!File.Exists($"{itemmodPath}\\config.txt"))
            {
                string fullConfig =
@"# If set to true when getting milk/pizza it will not replace an existing item in your inventory
dont-replace-item=true

# Bypasses the client-side profanity filter
# Set to 'true' to allow swear words
profanity-bypass=true

# Full profanity bypass (Allowing anything to be sent in the chat)
full-profanity-bypass=false

# Disable jumpboost (B button)
disable-jumpboost=false

# Jumpboost height (default: 20)
jumpboost-height=20

# Discord webhook for #id dc (leave blank to disable)
webhook=

# Crab mode enabled or not?
crabmode_enabled=true

# Show ammo when equipping gun
showammo=true

# Should the command '#id dc' send the steamids as links or plain text?
dc_playerlist_haslinks=false

# Should you receive a warning in chat if you have done over 100 damage to somebody?
warn_on_100_damage=false

# Skip the Intro UI on round beginning?
skip_intro=true

# Show gamemode name in chat upon loading into map (For use with Disable IntroUI)
show_gamemode_id_in_chat=true

# Show gamemodes WaitingRoom and Practice? (For use with Disable IntroUI)
show_more_gamemode_ids_in_chat=true

# Tick the show box rates box by default
show_box_rates_by_default=false

# Should we hide the About button on the main menu?
hide_about_tab=true

# Send steamid when using #id to webhook (if set)
id_to_webhook=true

# Show the steamID in notepad
id_to_notepad=true

# Should the playerlist be sent to discord on round start?
playerlist_to_dc_auto=false

# Unban AK and Dual when you are host?
unban_ak_dual=true

# Auto copy steamid on #id ?
auto_copy_on_id=false

# Show count of how many people are spectating you
show_spectator_count=false

# Show spectator counter right below the crosshair? true=below crosshair false=top right
show_spectator_count_below_crosshair=false

# Show who people are spectating in TAB (may be buggy)
show_spectating_in_tab=false

# Should we hide unnessecary particles/go through objects?
clear_view=false

# Do you want to get local death messages? (For vanilla lobbys)
local_death_messages=false

# Do you want to have death messages in the lobbys you host?
host_death_messages=false

# Do you want to get local join/leave messages?
local_joinleave_messages=false

# Do you want to have join/leave messages in the lobbys you host?
host_joinleave_messages=false

# Do you want #steam command to open in the overlay instead?
steam_cmd_use_overlay=false

### KEYBINDS

key_pizza=112
key_milk=109
key_grenade=111
key_jumpboost=98
key_ready=107
key_spin=108
key_kill=290
key_checktag=117
key_crab=106
";

        File.WriteAllText($"{itemmodPath}\\config.txt", fullConfig);
            }

            loadConfig();
        }

        public static void loadConfig()
        {
            string conf = File.ReadAllText($"{itemmodPath}\\config.txt");
            var configSettings = new Dictionary<string, string>();
            bool newConfig = false;

            foreach (var line in conf.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.StartsWith("#"))
                {
                    var parts = line.Trim().Split('=');
                    if (parts.Length == 2)
                    {
                        configSettings.Add(parts[0].Trim(), parts[1].Trim());
                    }
                }
            }

            if (configSettings.TryGetValue("dont-replace-item", out string dontreplaceitem))
            {
                dontReplaceItem = bool.TryParse(dontreplaceitem, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("profanity-bypass", out string defaultValue2))
            {
                profanityBypass = bool.TryParse(defaultValue2, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("full-profanity-bypass", out string full_profanity_bypass))
            {
                fullprofanityBypass = bool.TryParse(full_profanity_bypass, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("disable-jumpboost", out string defaultValue5))
            {
                jumpboostdisabled = bool.TryParse(defaultValue5, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("jumpboost-height", out string defaultValue1))
            {
                jumpboostamount = float.TryParse(defaultValue1, out float result) ? result : 20;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("webhook", out string webhookValue))
            {
                if (webhookValue.StartsWithMod("http")) webhook = webhookValue;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("crabmode_enabled", out string crabmodeenabled))
            {
                crabModeIsOn = bool.TryParse(crabmodeenabled, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("showammo", out string showammo))
            {
                show_ammo = bool.TryParse(showammo, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("dc_playerlist_haslinks", out string dc_playerlist_haslinks))
            {
                dcplayerlisthaslinks = bool.TryParse(dc_playerlist_haslinks, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("warn_on_100_damage", out string warn_on_100_damage))
            {
                warn100dmg = bool.TryParse(warn_on_100_damage, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("skip_intro", out string skip_intro))
            {
                skipIntro = bool.TryParse(skip_intro, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("show_gamemode_id_in_chat", out string show_gamemode_id_in_chat))
            {
                show_gameid_chat = bool.TryParse(show_gamemode_id_in_chat, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("show_more_gamemode_ids_in_chat", out string show_more_gamemode_ids_in_chat))
            {
                show_moregameid_chat = bool.TryParse(show_more_gamemode_ids_in_chat, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("show_box_rates_by_default", out string box_rates))
            {
                showBoxRates = bool.TryParse(box_rates, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("hide_about_tab", out string hide_about_tab))
            {
                hideAbout = bool.TryParse(hide_about_tab, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("id_to_webhook", out string id_to_webhook))
            {
                IdToWebhook = bool.TryParse(id_to_webhook, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("id_to_notepad", out string id_to_notepad))
            {
                IdToNotepad = bool.TryParse(id_to_notepad, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("playerlist_to_dc_auto", out string playerlist_to_dc_auto))
            {
                playerlisttodcauto = bool.TryParse(playerlist_to_dc_auto, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("unban_ak_dual", out string unban_ak_dual))
            {
                unbanAkDual = bool.TryParse(unban_ak_dual, out bool result) ? result : true;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("auto_copy_on_id", out string auto_copy_on_id))
            {
                autoCopyOnId = bool.TryParse(auto_copy_on_id, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("show_spectator_count", out string show_spectator_count))
            {
                showSpectatorCount = bool.TryParse(show_spectator_count, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("show_spectator_count_below_crosshair", out string show_spectator_count_below_crosshair))
            {
                spectatorCountBelowCrosshair = bool.TryParse(show_spectator_count_below_crosshair, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("show_spectating_in_tab", out string show_spectating_in_tab))
            {
                spectatorsInTAB = bool.TryParse(show_spectating_in_tab, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("clear_view", out string clear_view))
            {
                clearView = bool.TryParse(clear_view, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("local_death_messages", out string local_death_messages))
            {
                localdeathmessages = bool.TryParse(local_death_messages, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("host_death_messages", out string host_death_messages))
            {
                hostdeathmessages = bool.TryParse(host_death_messages, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("local_joinleave_messages", out string local_joinleave_messages))
            {
                localjoinleavemessages = bool.TryParse(local_joinleave_messages, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("host_joinleave_messages", out string host_joinleave_messages))
            {
                hostjoinleavemessages = bool.TryParse(host_joinleave_messages, out bool result) ? result : false;
            }
            else newConfig = true;

            if (configSettings.TryGetValue("steam_cmd_use_overlay", out string steam_cmd_use_overlay))
            {
                steamcmduseoverlay = bool.TryParse(steam_cmd_use_overlay, out bool result) ? result : false;
            }
            else newConfig = true;

            /// ----------------------------------- KEYBINDS -------------------------------------------------------------------------------

            if (configSettings.TryGetValue("key_pizza", out string defaultValue7))
            {
                key_pizza = int.TryParse(defaultValue7, out int result) ? result : 112;
            }

            if (configSettings.TryGetValue("key_milk", out string defaultValue8))
            {
                key_milk = int.TryParse(defaultValue8, out int result) ? result : 109;
            }

            if (configSettings.TryGetValue("key_grenade", out string defaultValue9))
            {
                key_grenade = int.TryParse(defaultValue9, out int result) ? result : 111;
            }

            if (configSettings.TryGetValue("key_jumpboost", out string defaultValue10))
            {
                key_jumpboost = int.TryParse(defaultValue10, out int result) ? result : 98;
            }

            if (configSettings.TryGetValue("key_ready", out string defaultValue11))
            {
                key_ready = int.TryParse(defaultValue11, out int result) ? result : 107;
            }

            if (configSettings.TryGetValue("key_spin", out string defaultValue12))
            {
                key_spin = int.TryParse(defaultValue12, out int result) ? result : 108;
            }

            if (configSettings.TryGetValue("key_kill", out string defaultValue13))
            {
                key_kill = int.TryParse(defaultValue13, out int result) ? result : 290;
            }

            if (configSettings.TryGetValue("key_checktag", out string defaultValue15))
            {
                key_checktag = int.TryParse(defaultValue15, out int result) ? result : 117;
            }

            if (configSettings.TryGetValue("key_crab", out string defaultValue23))
            {
                key_crab = int.TryParse(defaultValue23, out int result) ? result : 291;
            }

            string confver = "";
            try
            {
                confver = File.ReadAllText($"{itemmodPath}\\.config-ver");
            }
            catch { }

            if (confver != apiver.ToString() && newConfig)
            {
                printNotepad("ItemMOD has new config available!\nTo refresh your config file please do the following:\n\n1) Open Crab Game files\n2) Go to BepInEx\\plugins\\ItemMod\n3) Delete or rename config.txt\n4) Launch and close Crab Game\n5) Open the new file and make your configurations\n\nThis message will not be shown again.");
                File.WriteAllText($"{itemmodPath}\\.config-ver", apiver.ToString());
            }
            else if (confver != apiver.ToString() && !newConfig)
            {
                File.WriteAllText($"{itemmodPath}\\.config-ver", apiver.ToString());
            }
        }

        public class Main : MonoBehaviour
        {
            void Awake() // called once per round
            {
                playerDMG.Clear();
                itemToGive = -1;
                spectatingME.Clear();
                spectatorsList.Clear();

                if (LobbyManager.Instance.gameMode.id == 0 || LobbyManager.Instance.gameMode.id == 13)
                {
                    gameStarted = false;
                }
                else
                {
                    gameStarted = true;
                }

                if (playerlisttodcauto && LobbyManager.Instance.gameMode.id != 13)
                {
                    if (string.IsNullOrWhiteSpace(webhook))
                    {
                        playerlisttodcauto = false;
                    }
                    else
                    {
                        sendUserListToWebhook();
                    }
                }

                doClearView();
            }

            void Update()
            {
                // dont register keybinds when typing in chat
                if (ChatBox.Instance.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().isFocused) return;

                if (Input.GetKeyDown((KeyCode)key_pizza))
                {
                    itemToGive = 12;
                }

                if (Input.GetKeyDown((KeyCode)key_milk))
                {
                    itemToGive = 11;
                }

                if (Input.GetKeyDown((KeyCode)key_grenade))
                {
                    itemToGive = 13;
                }

                if (Input.GetKeyDown((KeyCode)key_ready))
                {
                    ClientSend.TryInteract(4);
                }

                if (Input.GetKeyDown((KeyCode)key_spin))
                {
                    totalReloadTime += 1f;
                    ClientSend.PlayerReload(totalReloadTime);
                }

                if (Input.GetKeyDown((KeyCode)key_kill))
                {
                    ClientSend.PlayerDied(clientId, Vector3.zero);
                }

                if (Input.GetKeyDown((KeyCode)key_crab))
                {
                    if (crabModeIsOn && crabModeAllowed)
                    {
                        crabMode = !crabMode;
                        
                        if (crabMode) ForceMessageModGreen("You have become a crab :D");
                        else ForceMessageModRed("You are no longer a crab.");
                        
                        PlayerServerCommunication.Instance.ForceMovementUpdate();
                    }
                }

                if (!jumpboostdisabled && Input.GetKeyDown((KeyCode)key_jumpboost))
                {
                    if (LobbyManager.Instance.gameMode.id == 13 || LobbyManager.Instance.gameMode.id == 0)
                    {
                        GameObject clientObject = GameObject.Find("/Player");
                        PlayerMovement clientMovement = clientObject?.GetComponent<PlayerMovement>();

                        clientMovement.PushPlayer(new Vector3(0, jumpboostamount, 0));
                    }
                }

                if (Input.GetKeyDown((KeyCode)key_checktag))
                {
                    if (LobbyManager.Instance.gameMode.id == 4) // Only for tag
                    {
                        try
                        {
                            GameModeTag gmtag;
                            try
                            {
                                gmtag = GameObject.Find("/GameManager (1)").GetComponent<GameModeTag>();
                            }
                            catch
                            {
                                gmtag = GameObject.Find("/GameManager").GetComponent<GameModeTag>();
                            }
                            if (gmtag.field_Private_List_1_UInt64_0.Contains(clientId))
                            {
                                ForceMessageModRed("You are tagged!");
                            }
                            else
                            {
                                ForceMessageModGreen("You are not tagged.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning(ex.ToString());
                        } 
                    }
                }
            }
        }

        public static void printNotepad(string text)
        {
            File.WriteAllText($"{itemmodPath}\\ItemMOD-message", text);
            Process.Start("notepad.exe", $"{itemmodPath}\\ItemMOD-message");
        }

        public static void openNotepad(string path)
        {
            if (File.Exists("C:\\Program Files\\Notepad++\\notepad++.exe"))
            {
                Process.Start("C:\\Program Files\\Notepad++\\notepad++.exe", $"\"{path}\"");
            }
            else
            {
                Process.Start("notepad.txt", $"\"{path}\"");
            }
        }

        public static void doClearView()
        {
            if (!clearView) return;

            //bool unfair = false;
            switch (LobbyManager.Instance.map.id)
            {
                case 5:
                case 17:
                case 22:
                    turnOffObject("Statue/StatueFinal/Creepy"); // Annoying Sounds

                    turnOffObject("Grass");
                    turnOffObject("r"); // House
                    turnOffObject("r (1)");
                    turnOffObject("r (2)");
                    if (LobbyManager.Instance.map.id == 17)
                    {
                        turnOffObject("plainsGround/Plane.000");
                        turnOffObject("plainsGround/Plane.001");
                    }
                    else
                    {
                        turnOffObject("Cube (2)");
                    }
                    break;

                case 10:
                    turnOffObject("Bush_Snow_2");
                    turnOffObject("Bush_Snow_2 (1)");
                    turnOffObject("Bush_Snow_2 (2)");
                    turnOffObject("Bush_Snow_2 (3)");
                    turnOffObject("Bush_Snow_2 (4)");
                    turnOffObject("Bush_Snow_2 (5)");
                    turnOffObject("Bush_Snow_2 (6)");
                    turnOffObject("Bush_Snow_2 (7)");
                    break;

                case 20:
                    turnOffObject("DamageNumbers"); // Maplayout credit text
                    break;
                case 29:
                    turnOffObject("Cube"); // random pole in the distance with no collision
                    turnOffObject("Bush_Snow_2");
                    turnOffObject("Bush_Snow_2 (1)");
                    turnOffObject("Bush_Snow_2 (2)");
                    turnOffObject("Bush_Snow_2 (3)");
                    turnOffObject("Bush_Snow_2 (4)");
                    turnOffObject("Bush_Snow_2 (5)");
                    turnOffObject("Bush_Snow_2 (6)");
                    turnOffObject("Bush_Snow_2 (7)");
                    turnOffObject("Bush_Snow_2 (8)");
                    turnOffObject("Bush_Snow_2 (9)");
                    turnOffObject("Bush_Snow_2 (10)");
                    turnOffObject("Bush_Snow_2 (11)");
                    turnOffObject("Bush_Snow_2 (12)");
                    turnOffObject("Bush_Snow_2 (13)");
                    turnOffObject("Bush_Snow_2 (14)");
                    turnOffObject("Bush_Snow_2 (15)");
                    turnOffObject("Bush_Snow_2 (16)");
                    turnOffObject("Bush_Snow_2 (17)");
                    turnOffObject("Bush_Snow_2 (18)");
                    turnOffObject("Bush_Snow_2 (19)");
                    turnOffObject("Bush_Snow_2 (20)");
                    break;

                case 32:
                    turnOffObject("WoodLog (3)"); // the reandom log with no collision
                    break;

                case 33:
                    turnOffObject("Dust");
                    turnOffObject("ScrollController"); // ground
                    removeRenderer("Map"); // disable bcs fog
                    break;
                case 39:
                    turnOffObject("Cactus_3 (3)");
                    turnOffObject("Cactus_3 (4)");
                    break;

                case 42:
                case 43:
                    turnOffObject("Bush_Snow_2");
                    turnOffObject("Bush_Snow_2 (1)");
                    turnOffObject("Bush_Snow_2 (2)");
                    turnOffObject("Bush_Snow_2 (3)");
                    turnOffObject("Bush_Snow_2 (4)");
                    turnOffObject("Bush_Snow_2 (5)");
                    break;
                case 56:
                    turnOffObject("Ball");
                    turnOffObject("Ball (1)");
                    break;

                case 57:
                    turnOffObject("Bush_Snow_2");
                    turnOffObject("Bush_Snow_2 (1)");
                    turnOffObject("Bush_Snow_2 (2)");
                    turnOffObject("Bush_Snow_2 (3)");
                    turnOffObject("Bush_Snow_2 (4)");
                    turnOffObject("Bush_Snow_2 (5)");
                    turnOffObject("Bush_Snow_2 (6)");
                    turnOffObject("Bush_Snow_2 (7)");
                    turnOffObject("Bush_Snow_2 (8)");
                    turnOffObject("Bush_Snow_2 (9)");
                    turnOffObject("Bush_Snow_2 (10)");
                    turnOffObject("Bush_Snow_2 (11)");
                    turnOffObject("Bush_Snow_2 (12)");
                    turnOffObject("Bush_Snow_2 (13)");
                    turnOffObject("Bush_Snow_2 (14)");
                    turnOffObject("Bush_Snow_2 (15)");
                    turnOffObject("Bush_Snow_2 (16)");
                    turnOffObject("Bush_Snow_2 (17)");
                    turnOffObject("Bush_Snow_2 (18)");
                    turnOffObject("Bush_Snow_2 (19)");
                    turnOffObject("Bush_Snow_2 (20)");
                    turnOffObject("Bush_Snow_2 (21)");
                    turnOffObject("Bush_Snow_2 (22)");
                    turnOffObject("Bush_Snow_2 (23)");
                    turnOffObject("Bush_Snow_2 (24)");
                    turnOffObject("Bush_Snow_2 (25)");
                    turnOffObject("Bush_Snow_2 (26)");
                    break;

                default:
                    break;
            }
            
            // Finally, we turn off snow and windparticles for every map
            turnOffObject("Snow");
            turnOffObject("WindParticles");
        }

        public static void turnOffObject(string obj)
        {
            try
            {
                GameObject gameObject = GameObject.Find(obj);
                gameObject.active = false;
                gameObject.SetActive(false);
            }
            catch { }
        }

        public static void removeRenderer(string obj)
        {
            try
            {
                GameObject gameObject = GameObject.Find(obj);
                UnityEngine.Object.Destroy(gameObject.GetComponent<UnityEngine.MeshRenderer>());
            }
            catch { }
        }

        public static string GetPlayerSteamId(int playerNumber)
        {
            playersList = GameManager.Instance.activePlayers.entries.ToList();
            for (int u = 0; u <= playersList.Count; u++)
            {
                try
                {
                    if (playersList[u].value.playerNumber == playerNumber)
                    {
                        return playersList[u].value.steamProfile.m_SteamID.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error[GetPlayerSteamId] : " + ex.Message);
                }
            }
            return null;
        }

        public static int GetPlayerNumber(ulong steamId)
        {
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
            {
                if (player.Value.steamProfile.m_SteamID == steamId)
                {
                    return player.value.playerNumber;
                }
            }

            return 0;
        }

        public static string GetPlayerSteamName(string steamid)
        {
            return SteamFriends.GetFriendPersonaName((CSteamID)ulong.Parse(steamid));
        }
        
        public static string GetPlayerSteamName(ulong steamid)
        {
            return SteamFriends.GetFriendPersonaName((CSteamID)steamid);
        }

        public static string GetPlayerSteamName(CSteamID steamid)
        {
            return SteamFriends.GetFriendPersonaName(steamid);
        }

        public static void SendMessage(string message)
        {
            ChatBox.Instance.SendMessage(message);
        }

        public static void ForceMessage(string message)
        {
            ChatBox.Instance.ForceMessage(message);
        }
        public static void ForceMessageModWhite(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#FFFFFF>" + message + "</color>");
        }
        public static void ForceMessageModRed(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#ff0000>" + message + "</color>");
        }
        public static void ForceMessageModGreen(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#00ff00>" + message + "</color>");
        }
        public static void ForceMessageModYellow(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#ffff00>" + message + "</color>");
        }
        public static void ForceMessageModOrange(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#FFAC00>" + message + "</color>");
        }
        public static void ForceMessageModBlue(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#0000ff>" + message + "</color>");
        }
        public static void ForceMessageModLightBlue(string message)
        {
            ChatBox.Instance.ForceMessage("<color=#17b54b>[ItemMOD]</color> <color=#94EBFF>" + message + "</color>");
        }
        public static void SendServerMessage(string message)
        {
            ServerSend.SendChatMessage(1, message);
        }

        public static int weaponNameToId(string name)
        {
            switch (name.ToLower())
            {
                case "ak":
                    return 0;
                case "glock":
                case "pistol":
                    return 1;
                case "revolver":
                    return 2;
                case "dual":
                case "shotgun":
                    return 3;
                case "bat":
                    return 4;
                case "bomb":
                    return 5;
                case "katana":
                    return 6;
                case "knife":
                    return 7;
                case "pipe":
                    return 8;
                case "snowball":
                    return 9;
                case "stick":
                    return 10;
                case "milk":
                    return 11;
                case "pizza":
                    return 12;
                case "grenade":
                    return 13;

                default:
                    return -1;
            }
        }

        public static string weapnIdToName(int id)
        {
            switch (id)
            {
                case 0:
                    return "AK";
                case 1:
                    return "GLOCK";
                case 2:
                    return "REVOLVER";
                case 3:
                    return "DUAL";
                case 4:
                    return "BAT";
                case 5:
                    return "BOMB";
                case 6:
                    return "KATANA";
                case 7:
                    return "KNIFE";
                case 8:
                    return "PIPE";
                case 9:
                    return "SNOWBALL";
                case 10:
                    return "STICK";
                case 11:
                    return "MILK";
                case 12:
                    return "PIZZA";
                case 13:
                    return "GRENADE";

                default:
                    return "UNKNOWN";
            }
        }

        public static void broadcastBlue(string message, string name = "")
        {
            foreach (var player in LobbyManager.steamIdToUID.Keys)
            {
                try
                {
                    Packet packet = new Packet(2);
                    packet.Method_Public_Void_UInt64_0(player); // Id of Sender
                    packet.Method_Public_Void_String_0(name); // Name
                    packet.Method_Public_Void_String_0(message); // Message
                    ServerSend.Method_Private_Static_Void_UInt64_ObjectPublicIDisposableLi1ByInByBoUnique_0(player, packet);
                }
                catch { }
            }
        }

        public async static void sendwebhook(string webhook, string message)
        {
            string postdata = $"{{\"content\": \"{message}\"}}";
            HttpContent content = new StringContent(postdata, Encoding.UTF8, "application/json");
            try
            {
                await httpClient.PostAsync(webhook, content);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error[SendWebhook]: " + ex.Message);
            }
        }

        [HarmonyPatch(typeof(ClientSend), nameof(ClientSend.DamagePlayer))]
        [HarmonyPrefix]
        public static void playerDamage(ulong param_0, int param_1, Vector3 param_2, int param_3, int param_4)
        {
            if (param_0 != clientId && warn100dmg && LobbyManager.Instance.gameMode.id != 0 && LobbyManager.Instance.gameMode.id != 13)
            {
                if (!playerDMG.TryGetValue(param_0, out int currentHP))
                {
                    playerDMG[param_0] = 0; // Initialize with default HP if it doesnt exist
                }

                switch (param_3)
                {
                    case 1: // glock
                        playerDMG[param_0] += 15;
                        break;
                    case 4: // bat
                        playerDMG[param_0] += 12;
                        break;
                    case 6: // katana
                        playerDMG[param_0] += 18;
                        break;
                    case 7: // knife
                        playerDMG[param_0] += 12;
                        break;
                    case 8: // pipe
                        playerDMG[param_0] += 20;
                        break;
                }

                int value = playerDMG[param_0];

                if (value > 100)
                {
                    ForceMessageModRed($"{GetPlayerSteamName(param_0.ToString())} took {value} dmg from you!"); 
                }
            }


            return;
        }

        [HarmonyPatch(typeof(ClientSend), nameof(ClientSend.PlayerRotation))]
        [HarmonyPrefix]
        public static void playerRot(ref float param_0, float param_1, ulong param_2)
        {
            if (crabMode)
            {
                param_0 = -180;
            }

            return;
        }

        public static void quitChat()
        {
            ChatBox.Instance.field_Private_Boolean_0 = false; // typing boolean
            ChatBox.Instance.inputField.text = "";
            ChatBox.Instance.inputField.interactable = false;
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.Instance.AppendMessage))]
        [HarmonyPostfix]
        public static void chatreceive(ulong param_1, string param_2, string param_3)
        {
            if (param_2 == "!demon")
            {
                ForceMessageModRed("Crab mode disabled.");

                crabModeAllowed = false;
                crabMode = false;
            }
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.Instance.SendMessage))]
        [HarmonyPrefix]
        public static bool chatsend(string param_1)
        {
            if (param_1.StartsWithMod("#id"))
            {
                if (string.IsNullOrWhiteSpace(param_1.Substring(3)) || string.IsNullOrWhiteSpace(param_1.Substring(4)))
                {
                    quitChat();
                    return false;
                }

                string working_string = param_1.Substring(4);
                if (working_string.StartsWith("#"))
                {
                    working_string = working_string.Substring(1);
                }

                if (working_string == "all")
                {
                    playersList = GameManager.Instance.activePlayers.entries.ToList();

                    //string message = $"Lobby id: {SteamManager.Instance.currentLobby.m_SteamID}\nPlayers: {GameManager.Instance.activePlayers.Count + GameManager.Instance.spectators.Count}\n\n";
                    string message = $"Lobby id: {SteamManager.Instance.currentLobby.m_SteamID}\nLobby name: {SteamworksNative.SteamMatchmaking.GetLobbyData(SteamManager.Instance.currentLobby, "LobbyName")}\nPlayers: {GameManager.Instance.activePlayers.Count}+{GameManager.Instance.spectators.Count} ({GameManager.Instance.activePlayers.Count + GameManager.Instance.spectators.Count})\nGamemode: {LobbyManager.Instance.gameMode.modeName}\nMap: {LobbyManager.Instance.map.mapName}\n\n";

                    if (dcplayerlisthaslinks) // Obviously the best way to do it what are you talking about?
                    {
                        foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
                        {
                            message += $"{GetPlayerSteamName(player.Value.steamProfile.m_SteamID)} ({player.value.playerNumber}) : <https://steamcommunity.com/profiles/{player.Value.steamProfile.m_SteamID}>\n";
                        }
                        try
                        {
                            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.spectators)
                            {
                                message += $"[SPECTATOR] {GetPlayerSteamName(player.Value.steamProfile.m_SteamID)}: <https://steamcommunity.com/profiles/{player.Value.steamProfile.m_SteamID}>\n";
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
                        {
                            message += $"{GetPlayerSteamName(player.Value.steamProfile.m_SteamID)} ({player.value.playerNumber}) : {player.Value.steamProfile.m_SteamID}\n";
                        }
                        try
                        {
                            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.spectators)
                            {
                                message += $"[SPECTATOR] {GetPlayerSteamName(player.Value.steamProfile.m_SteamID)}: {player.Value.steamProfile.m_SteamID}\n";
                            }
                        }
                        catch { }
                    }

                    printNotepad(message);
                }
                else if (working_string == "dc")
                {
                    sendUserListToWebhook();
                    quitChat();
                    return false;
                }
                else
                {
                    string steamid = GetPlayerSteamId(int.Parse(working_string));
                    
                    if (string.IsNullOrWhiteSpace(steamid))
                    {
                        ForceMessageModRed("Invalid player or Unable to get steamid");
                        
                        quitChat();
                        return false;
                    }
                    
                    string username = GetPlayerSteamName(steamid);

                    ForceMessageModWhite($"Steam ID of #{working_string} {username} : {steamid}");

                    if (autoCopyOnId)
                    {
                        GUIUtility.systemCopyBuffer = steamid;
                        //ForceMessageMod("Copied!");
                    }

                    if (IdToNotepad)
                    {
                        printNotepad($"Steam ID of #{working_string} {username} : {steamid}");
                    }

                    if (IdToWebhook)
                    {
                        if (string.IsNullOrWhiteSpace(webhook))
                        {
                            IdToWebhook = false;
                        }
                        else
                        {
                            sendwebhook(webhook, $"Steam ID of #{working_string} {username} : {steamid}");
                        }
                    }

                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#steam"))
            {
                bool reverse = false;
                if (param_1.Substring(6).StartsWith('2')) reverse = true;
                if (string.IsNullOrWhiteSpace(param_1.Substring(6)) || string.IsNullOrWhiteSpace(param_1.Substring(7)))
                {
                    quitChat();
                    return false;
                }

                string working_string = param_1.Substring(7);
                if (reverse) working_string = working_string.Substring(1);
                if (working_string.StartsWith("#"))
                {
                    working_string = working_string.Substring(1);
                }

                string steamid = GetPlayerSteamId(int.Parse(working_string));

                if (string.IsNullOrWhiteSpace(steamid))
                {
                    ForceMessageModRed("Invalid player or Unable to get steamid");

                    quitChat();
                    return false;
                } else
                {
                    ForceMessageModGreen($"Opening player #{working_string}'s Steam profile...");
                    if (!reverse)
                    {
                        if (steamcmduseoverlay)
                        {
                            if (ulong.TryParse(steamid, out ulong steamidUlong))
                            {
                                SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(steamidUlong));
                            }
                        }
                        else
                        {
                            Process.Start("steam://url/SteamIDPage/" + steamid);
                        }
                    }
                    else
                    {
                        if (!steamcmduseoverlay)
                        {
                            if (ulong.TryParse(steamid, out ulong steamidUlong))
                            {
                                SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(steamidUlong));
                            }
                        }
                        else
                        {
                            Process.Start("steam://url/SteamIDPage/" + steamid);
                        }
                    }
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#win"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");
                    quitChat();
                    return false;
                }

                string workingstring = param_1.ToLower().Substring(4);

                if (workingstring == " skip")
                {
                    if (SteamManager.Instance.IsLobbyOwner())
                    {
                        ServerSend.LoadMap(6, 0);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(workingstring))
                {
                    if (workingstring.StartsWith("#")) { workingstring = workingstring.Substring(1); }
                    string[] parts;
                    if (workingstring.Contains(','))
                    {
                        ForceMessageModYellow("New! You can do #win 1 1337");
                        parts = workingstring.Split(',');
                    } else
                    {
                        parts = workingstring.Split(' ');
                    }
                    
                    if (parts.Length == 2)
                    {
                        ulong money;

                        if (parts[1] == "max")
                        {
                            money = UInt64.MaxValue;
                        } else
                        {
                            if (!ulong.TryParse(parts[1].Trim(), out money))
                            {
                                ForceMessageModRed("Invalid money amount.");

                                quitChat();
                                return false;
                            }
                        }
                        
                        if (!int.TryParse(parts[0].Trim(), out int playerid))
                        {
                            ForceMessageModRed("Invalid player.");

                            quitChat();
                            return false;
                        }

                        ulong steamid = ulong.Parse(GetPlayerSteamId(playerid));
                        ServerSend.SendWinner(steamid, money);
                    }
                    else
                    {
                        ulong steamid = ulong.Parse(GetPlayerSteamId(int.Parse(workingstring)));
                        ServerSend.SendWinner(steamid, ulong.MaxValue);
                    }
                }
                else
                {
                    ServerSend.SendWinner(clientId, ulong.MaxValue);
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#give"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");
                    quitChat();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(param_1.Substring(5)) || string.IsNullOrWhiteSpace(param_1.Substring(6)))
                {
                    quitChat();
                    return false;
                }

                string working_string = param_1.Substring(6);
                string[] parts;

                if (working_string.Contains(','))
                {
                    ForceMessageModYellow("New! You can do #give 1 dual 999");
                    parts = working_string.Split(',');
                } else
                {
                    parts = working_string.Split(' ');
                }

                if (parts.Length != 3)
                {
                    ForceMessageModRed("Invalid format.");
                    ForceMessageModRed("Expected 3 values.");
                    ForceMessageModRed("#give 1 dual 999");

                    quitChat();
                    return false;
                }

                if (parts[0].StartsWith('#'))
                {
                    parts[0] = parts[0].Substring(1);
                }

                int player, weapon = -1, ammo;

                weapon = weaponNameToId(parts[1]);

                if (weapon == -1)
                {
                    if (!int.TryParse(parts[1].Trim(), out weapon))
                    {
                        ForceMessageModRed("Invalid weapon value. (id or name)");

                        quitChat();
                        return false;
                    }
                }

                if (parts[2] == "max")
                {
                    ammo = Int32.MaxValue;
                }
                else
                {
                    if (!int.TryParse(parts[2].Trim(), out ammo))
                    {
                        ForceMessageModRed("Invalid ammo value. (number)");

                        quitChat();
                        return false;
                    }
                }
                

                if (parts[0] == "*")
                {
                    playersList = GameManager.Instance.activePlayers.entries.ToList();

                    foreach (var entry in playersList)
                    {
                        try
                        {
                            ServerSend.DropItem(entry.value.steamProfile.m_SteamID, weapon, SharedObjectManager.Instance.GetNextId(), ammo);
                        }
                        catch { }
                    }

                    quitChat();
                    return false;
                }

                if (!int.TryParse(parts[0].Trim(), out player))
                {
                    ForceMessageModRed("Invalid player value. (#1 or *)");
                }
                else
                {
                    if (!ulong.TryParse(GetPlayerSteamId(player), out ulong steamid))
                    {
                        ForceMessageModRed("Unable to get steamid of player.");
                        quitChat();
                        return false;
                    }

                    ServerSend.DropItem(steamid, weapon, SharedObjectManager.Instance.GetNextId(), ammo);
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#money"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");
                    quitChat();
                    return false;
                }

                int player = -1;
                ulong steamid = 0;

                try
                {
                    string comparestring = param_1.Substring(7);
                    if (!string.IsNullOrEmpty(comparestring))
                    {
                        if (comparestring.StartsWith("#"))
                        {
                            comparestring = comparestring.Substring(1);
                        }

                        if (comparestring == "*")
                        {
                            player = -2;
                        }
                        else
                        {
                            player = int.Parse(comparestring);

                        }
                    }
                }
                catch { }

                if (player != -1)
                {
                    try
                    {
                        steamid = ulong.Parse(GetPlayerSteamId(player));
                    }
                    catch { } 
                }
                
                if (player == -2)
                {
                    playersList = GameManager.Instance.activePlayers.entries.ToList();
                    for (int u = 0; u <= playersList.Count; u++)
                    {
                        try
                        {
                            ServerSend.DropMoney(playersList[u].value.steamProfile.m_SteamID, 69, SharedObjectManager.Instance.GetNextId());
                        }
                        catch { }
                    }
                }
                else
                {
                    if (player == -1 || steamid == 0)
                    {
                        ServerSend.DropMoney(clientId, 69, SharedObjectManager.Instance.GetNextId());
                    } else
                    {
                        ServerSend.DropMoney(steamid, 69, SharedObjectManager.Instance.GetNextId());
                    }
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#dailytime"))
            {
                SaveManager.Instance.state.nextQuestAvailableTime = Il2CppSystem.DateTime.Now;
                SaveManager.Instance.Save();

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#daily"))
            {
                Quests.Instance.CompleteQuest();
                SaveManager.Instance.state.AddQuestProgress(187);
                SaveManager.Instance.Save();

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#rename"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");

                    quitChat();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(param_1.Substring(7)) || string.IsNullOrWhiteSpace(param_1.Substring(8)))
                {
                    quitChat();
                    return false;
                }

                string newname = param_1.Substring(8);

                try
                {
                    SteamworksNative.SteamMatchmaking.SetLobbyData(SteamManager.Instance.currentLobby, "LobbyName", newname);
                    ForceMessageModGreen("Lobby name succesfully set.");
                }
                catch (Exception ex)
                {
                    ForceMessageModRed("An error has occurred.");
                    Logger.LogWarning("Error[LobbyRename]: " + ex.Message);
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#bc"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");

                    quitChat();
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(param_1.Substring(3)))
                {
                    SendServerMessage(param_1.Substring(4));
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#blue"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");

                    quitChat();
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(param_1.Substring(5)))
                {
                    string[] parts = param_1.Substring(6).Split(',');

                    if (parts.Length == 1)
                    {
                        broadcastBlue(parts[0]);
                    }
                    else if (parts.Length == 2)
                    {
                        broadcastBlue(parts[1], parts[0]);
                    }
                }

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#kill"))
            {
                if (!SteamManager.Instance.IsLobbyOwner())
                {
                    ForceMessageModRed("You need to be the host.");

                    quitChat();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(param_1.Substring(5)) || string.IsNullOrWhiteSpace(param_1.Substring(6)))
                {
                    quitChat();
                    return false;
                }

                string working_string = param_1.Substring(6);
                if (working_string.StartsWith("#"))
                {
                    working_string = working_string.Substring(1);
                }

                string playerid = GetPlayerSteamId(int.Parse(working_string));
                
                if (string.IsNullOrWhiteSpace(playerid))
                {
                    ForceMessageModRed("Unable to find player.");

                    quitChat();
                    return false;
                }

                ulong steamid = ulong.Parse(playerid);
                ServerSend.PlayerDied(steamid, steamid, Vector3.zero);

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#conf"))
            {
                openNotepad($"{itemmodPath}\\config.txt");
                ForceMessageModGreen("Opening configuration window.");

                quitChat();
                return false;
            }

            if (param_1.StartsWithMod("#reloadconf"))
            {
                ForceMessageModGreen("Reloading config.");
                loadConfig();

                quitChat();
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.Awake))]
        [HarmonyPostfix]
        public static void chatboxawake()
        {
            if (profanityBypass)
            {
                if (fullprofanityBypass)
                {
                    ChatBox.Instance.field_Private_List_1_String_0.Clear();
                    return;
                }

                ChatBox.Instance.field_Private_List_1_String_0.Remove("bitch");
                ChatBox.Instance.field_Private_List_1_String_0.Remove("fuck");
                ChatBox.Instance.field_Private_List_1_String_0.Remove("shit");
                ChatBox.Instance.field_Private_List_1_String_0.Remove("sh1t");
            }
        }
        
        public static void sendUserListToWebhook()
        {
            // this method can activate whilst in loading screen thats why the try catch
            if (string.IsNullOrWhiteSpace(webhook))
            {
                try
                {
                    ForceMessageModRed("Error: webhook not set.");
                } catch { }
                return;
            }
            else
            {
                try
                {
                    ForceMessageModGreen("Sending playerlist to webhook...");
                } catch { }
            }

            string message = $"Lobby id: {SteamManager.Instance.currentLobby.m_SteamID}\\nLobby name: {SteamworksNative.SteamMatchmaking.GetLobbyData(SteamManager.Instance.currentLobby, "LobbyName")}\\nPlayers: {GameManager.Instance.activePlayers.Count}+{GameManager.Instance.spectators.Count} ({GameManager.Instance.activePlayers.Count + GameManager.Instance.spectators.Count})\\nGamemode: {LobbyManager.Instance.gameMode.modeName}\\nMap: {LobbyManager.Instance.map.mapName}\\n\\n";

            if (dcplayerlisthaslinks) // Obviously the best way to do it what are you talking about?
            {
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
                {
                    message += $"{GetPlayerSteamName(player.Value.steamProfile.m_SteamID)} ({player.value.playerNumber}) : <https://steamcommunity.com/profiles/{player.Value.steamProfile.m_SteamID}>\\n";
                }
                try
                {
                    foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.spectators)
                    {
                        message += $"[SPECTATOR] {GetPlayerSteamName(player.Value.steamProfile.m_SteamID)}: <https://steamcommunity.com/profiles/{player.Value.steamProfile.m_SteamID}>\\n";
                    }
                }
                catch { }
            }
            else
            {
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.activePlayers)
                {
                    message += $"{GetPlayerSteamName(player.Value.steamProfile.m_SteamID)} ({player.value.playerNumber}) : {player.Value.steamProfile.m_SteamID}\\n";
                }
                try
                {
                    foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> player in GameManager.Instance.spectators)
                    {
                        message += $"[SPECTATOR] {GetPlayerSteamName(player.Value.steamProfile.m_SteamID)}: {player.Value.steamProfile.m_SteamID}\\n";
                    }
                }
                catch { }
            }

            // Split the message into chunks of 2000 characters or less
            while (message.Length > 0)
            {
                string chunk = message.Substring(0, Math.Min(2000, message.Length));
                message = message.Substring(Math.Min(2000, message.Length));

                sendwebhook(webhook, chunk);
            }
        }

        [HarmonyPatch(typeof(ItemManager), nameof(ItemManager.Awake))]
        [HarmonyPostfix]
        internal static void PostItemManagerAwake()
        {
            if (SteamManager.Instance.IsLobbyOwner() && unbanAkDual)
            {
                ItemManager.idToItem[0].itemName = "AK";
                ItemManager.idToItem[3].itemName = "Dual Shotgun";
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.EquipItem))]
        [HarmonyPrefix]
        public static void equipitem(int param_1)
        {
            if (itemToGive == 11 || itemToGive == 12 || itemToGive == 13)
            {
                if (PlayerInventory.field_Private_Static_ArrayOf_ItemData_0[param_1] != null && dontReplaceItem &&
                    PlayerInventory.field_Private_Static_ArrayOf_ItemData_0[param_1] != ItemManager.GetItemById(11) &&
                    PlayerInventory.field_Private_Static_ArrayOf_ItemData_0[param_1] != ItemManager.GetItemById(12) &&
                    PlayerInventory.field_Private_Static_ArrayOf_ItemData_0[param_1] != ItemManager.GetItemById(13)
                    )
                {
                    ForceMessageModRed("You already have an item in that slot.");
                } else
                {
                    if (itemToGive == 12 || itemToGive == 11 || itemToGive == 13)
                    {
                        PlayerInventory.field_Private_Static_ArrayOf_ItemData_0[param_1] = ItemManager.GetItemById(itemToGive);
                        PlayerInventory.field_Private_Static_ArrayOf_ItemData_0[param_1].canDrop = true;
                    }
                }

                itemToGive = -1;
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.UpdateAmmoUI))]
        [HarmonyPrefix]
        public static void ammoui()
        {
            if (!show_ammo) { return; }

            try
            {
                GameObject ammo = GameObject.Find("GameUI/Status/BottomRight/Tab0/Ammo (1)");
                if (ammo == null)
                {
                    Logger.LogWarning("Didnt find ammo ui");
                } else
                {
                    ammo.active = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Error finding ammo ui: " + ex.ToString());
            }
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Instance.LeaveLobby))]
        [HarmonyPrefix]
        public static void leaveLobby()
        {
            createbuttonsez(true);
        }
        
        [HarmonyPatch(typeof(GameUiPause), nameof(GameUiPause.Awake))]
        [HarmonyPostfix]
        public static void gameUIPost()
        {
            createbuttonsez();
        }

        [HarmonyPatch(typeof(IntroUI), nameof(IntroUI.Start))]
        [HarmonyPrefix]
        public static void introUI()
        {
            if (skipIntro)
            {
                var camera = GameObject.Find("Camera");
                var mapcamera = GameObject.Find("MapCamera");
                var introui = GameObject.Find("IntroUI");
                introui.active = false;
                mapcamera.active = false;
                camera.active = true;
            }

            if (show_gameid_chat)
            {
                if (LobbyManager.Instance.gameMode.id == 0 || LobbyManager.Instance.gameMode.id == 13)
                {
                    if (show_moregameid_chat)
                    {
                        ForceMessageModWhite("Mode: " + LobbyManager.Instance.gameMode.modeName);
                    }
                }
                else
                {
                    ForceMessageModWhite("Mode: " + LobbyManager.Instance.gameMode.modeName);

                }
            }

        }

        [HarmonyPatch(typeof(GeneralUiButton), nameof(GeneralUiButton.OnPointerClick))]
        [HarmonyPrefix]
        public static void ButtonOnPointerClick(PointerEventData param_1)
        {
            try
            {
                if (showBoxRates)
                {
                    try
                    {
                        GameObject ratesdisplay = GameObject.Find("UI").transform.GetChild(18).gameObject;
                        ratesdisplay.active = true;
                    }
                    catch { }
                }

                createbuttonsez();

                string buttonName = param_1.pointerPress.transform.name;

                switch (param_1.pointerPress.transform.name)
                {
                    case "ItemMOD-iddc":
                        sendUserListToWebhook();
                        break;

                    case "ItemMOD-boxrates":
                        GameObject toggle = param_1.pointerPress.transform.FindChild("Toggle").gameObject;
                        toggle.active = !toggle.active;

                        GameObject ratesdisplay = GameObject.Find("UI").transform.GetChild(18).gameObject;
                        ratesdisplay.active = toggle.active;
                        break;

                    case "ItemMOD-247tag":
                        try
                        {
                            if (string.IsNullOrWhiteSpace(lobbyIDCheckURL) || !lobbyIDCheckURL.StartsWithMod("http"))
                            {
                                Prompt.Instance.NewPrompt("Error", "Unable to get lobby code.");
                            }
                            if (lobbyIDCheckURL.StartsWithMod("disable"))
                            {
                                Prompt.Instance.NewPrompt("Error", "Disabled.");
                                return;
                            }

                            string resultstring = "connection-error";

                            using (WebClient webClient = new WebClient())
                            {
                                resultstring = webClient.DownloadString(lobbyIDCheckURL);
                            }

                            string comparestring = resultstring.ToLower();

                            if (comparestring == "connection-error")
                            {
                                Prompt.Instance.NewPrompt("Error", "Unable to get lobby code.");
                                return;
                            }
                            else if (comparestring == "error")
                            {
                                Prompt.Instance.NewPrompt("Error", "An server error has occurred.\nUnable to get lobby code.");
                                return;
                            }
                            else if (comparestring == "none" || comparestring == "undefined" || comparestring == "0" || comparestring == "na")
                            {
                                Prompt.Instance.NewPrompt("Error", "Lobby code unknown.\nIs the lobby up?");
                                return;
                            }
                            else if (comparestring.StartsWith("custom"))
                            {
                                string[] error = resultstring.Split(";;;");
                                if (error.Length == 3)
                                {
                                    Prompt.Instance.NewPrompt(error[1], error[2]);
                                }
                                else
                                {
                                    Prompt.Instance.NewPrompt("Error", "An unknown error occurred.");
                                }

                                return;
                            }

                            ulong lobbyID = ulong.Parse(resultstring);

                            SteamManager.Instance.JoinLobby((CSteamID)lobbyID);
                        }
                        catch
                        {
                            Prompt.Instance.NewPrompt("Error.", "An unknown error has occurred.");
                        }
                        break;
                    case "ItemMOD-clipjoin":
                        string clip = TMPro.TMP_InputField.clipboard; // ClipBoard class causes exception luckily i found this lol

                        if (string.IsNullOrWhiteSpace(clip))
                        {
                            Prompt.Instance.NewPrompt("Error.", "Clipboard empty.");
                            return;
                        }

                        if (!ulong.TryParse(clip, out ulong lobbyid))
                        {
                            Prompt.Instance.NewPrompt("Error.", "Not a lobby code.");
                        }

                        SteamManager.Instance.JoinLobby((CSteamID)lobbyid);
                        break;
                    case "ItemMOD-Discord":
                        if (DiscordURL.StartsWithMod("http")) Process.Start(DiscordURL);
                        break;
                    case "ItemMOD-Website":
                        if (WebsiteURL.StartsWithMod("http")) Process.Start(WebsiteURL);
                        break;
                    case "ItemMOD-invrefresh":
                        SteamInventory.TryLoadInventory();
                        break;
                    case "ItemMOD-Config":
                        openNotepad($"{itemmodPath}\\config.txt");
                        break;
                    case "ItemMOD-ReloadConf":
                        loadConfig();
                        Prompt.Instance.NewPrompt("Done.", "Config reloaded.");
                        break;
                    case "ItemMOD-LogOutput":
                        openNotepad($"{crabgamePath}\\BepInEx\\LogOutput.log");
                        break;
                    case "ItemMOD-CrabDir":
                        Process.Start("explorer.exe", crabgamePath);
                        break;
                    case "ItemMOD-PluginsDir":
                        Process.Start("explorer.exe", pluginsPath);
                        break;


                    case "ItemMOD-update":
                        if (UpdateURL.StartsWithMod("http"))
                        {
                            Process.Start(UpdateURL);
                            UnityEngine.Application.Quit();
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GUI EXCEPTION: " + ex);
            }
        }

        [HarmonyPatch(typeof(GeneralUiButton), nameof(GeneralUiButton.OnPointerClick))]
        [HarmonyPostfix]
        public static void ButtonOnPointerClickPost(PointerEventData param_1)
        {
            //printCMD(param_1.pointerPress.transform.name);

            switch (param_1.pointerPress.transform.name)
            {
                case "Inventory":
                    GameObject tmp = GameObject.Find("UI/Inventory/GameSettingsWindow/Header/Tabs/ItemMOD-invrefresh");
                    if (tmp == null)
                    {
                        GameObject tabPrefab = GameObject.Find("UI/Inventory/GameSettingsWindow/Header/Tabs/TabUI (1)");
                        GameObject refreshButton = UnityEngine.Object.Instantiate<GameObject>(tabPrefab);
                        UnityEngine.Object.Destroy(refreshButton.GetComponent<UnityEngine.UI.Button>()); // Destroy that pesky original function
                        refreshButton.name = "ItemMOD-invrefresh";
                        refreshButton.transform.SetParent(GameObject.Find("UI/Inventory/GameSettingsWindow/Header/Tabs").transform);
                        refreshButton.transform.localPosition = new Vector3(404, 0, 0);
                        TextMeshProUGUI refreshbuttontext = refreshButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        refreshbuttontext.text = "Refresh";
                        refreshbuttontext.fontSizeMin = 15;
                    }
                    break;
                case "StartGame":
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);

                        try
                        {
                            GameObject slider = GameObject.Find("UI").transform.GetChild(3).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(3).GetChild(1).GetChild(0).gameObject;
                            slider.GetComponent<UnityEngine.UI.Slider>().maxValue = 99;
                            slider.GetComponent<UnityEngine.UI.Slider>().m_MaxValue = 99;
                        }
                        catch { }
                    });
                    break;
            }
        }

        public static bool buttonsExist = false;
        public static bool isInMainMenu = true;

        public static void createbuttonsez(bool delay = false)
        {
            SteamManager.Instance.StartCoroutine(createbuttons(delay).WrapToIl2Cpp());
        }

        public static System.Collections.IEnumerator createbuttons(bool delay)
        {
            if (delay) yield return new WaitForSeconds(1.0f);

            GameObject doesbuttonsexist = null;

            try {
                GameObject tmp = GameObject.Find("UI").gameObject;
                isInMainMenu = true;
            } catch { isInMainMenu = false; }

            if (isInMainMenu) { try { doesbuttonsexist = GameObject.Find("UI").transform.GetChild(2).GetChild(0).FindChild("ItemMOD-brand").gameObject; } catch { } }
            else { try { doesbuttonsexist = GameObject.Find("GameUI").transform.GetChild(10).GetChild(0).GetChild(2).GetChild(0).FindChild("ItemMOD-brand").gameObject; } catch { } }

            if (doesbuttonsexist == null) buttonsExist = false; else buttonsExist = true;
            
            if (!buttonsExist)
            {
                GameObject buttonPrefab = null;
                GameObject textPrefab = null;
                GameObject togglePrefab = null;
                GameObject questDisplay = null;

                if (isInMainMenu)
                {
                    try { togglePrefab = GameObject.Find("UI").transform.GetChild(12).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject; } catch { }

                    try {
                        buttonPrefab = GameObject.Find("UI").transform.GetChild(3).GetChild(0).GetChild(5).GetChild(0).GetChild(0).gameObject;
                        questDisplay = GameObject.Find("UI").transform.GetChild(2).GetChild(3).gameObject;
                    } catch { }
                } else {
                    try {
                        buttonPrefab = GameObject.Find("GameUI").transform.GetChild(10).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(2).gameObject;
                        questDisplay = GameObject.Find("GameUI").transform.GetChild(10).GetChild(0).GetChild(2).GetChild(2).gameObject;
                    } catch { }
                }

                try { textPrefab = buttonPrefab.transform.GetChild(4).gameObject; } catch { }

                try
                {
                    if (isInMainMenu) createtext(textPrefab, GameObject.Find("UI").transform.GetChild(2).GetChild(0).gameObject, "ItemMOD-brand", $"With ItemMOD v{ver}\nMade by Win_7", 12, new Vector3(170, -253, 0));
                    else createtext(textPrefab, GameObject.Find("GameUI").transform.GetChild(10).GetChild(0).GetChild(2).GetChild(0).gameObject, "ItemMOD-brand", $"With ItemMOD v{ver}\nMade by Win_7", 12, new Vector3(170, -253, 0));
                } catch { }

                if (buttonPrefab != null && questDisplay != null)
                {
                    try
                    {
                        if (!isInMainMenu && !string.IsNullOrWhiteSpace(webhook))
                        {
                            GameObject iddc = createbutton(buttonPrefab, questDisplay, "ItemMOD-iddc", "Playerlist to discord", new Vector3(-75, -175, 0));
                            iddc.GetComponent<RectTransform>().sizeDelta = new Vector2(151.9f, 45);
                        }
                    } catch { }
                }

                try
                {
                    GameObject dropdown = null;
                    GameObject newdropdown = null;
                    try
                    {
                        if (isInMainMenu) dropdown = GameObject.Find("UI").transform.GetChild(12).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;
                        else dropdown = GameObject.Find("GameUI").transform.GetChild(10).GetChild(0).GetChild(4).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;
                    }
                    catch { }
                    try
                    {
                        newdropdown = GameObject.Instantiate(dropdown);
                    }
                    catch { }

                    newdropdown.transform.parent = questDisplay.transform;
                    newdropdown.transform.localPosition = new Vector3(-285.5f, -172, 0);
                    newdropdown.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                    TMP_Dropdown dropdownobj = newdropdown.GetComponent<TMP_Dropdown>();
                    Il2CppSystem.Collections.Generic.List<TMP_Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<TMP_Dropdown.OptionData>();
                    options.Add(new TMP_Dropdown.OptionData("Complete daily"));
                    options.Add(new TMP_Dropdown.OptionData("Claim daily"));
                    options.Add(new TMP_Dropdown.OptionData("Get box"));
                    options.Add(new TMP_Dropdown.OptionData("Get box x3"));
                    options.Add(new TMP_Dropdown.OptionData("Do all"));
                    dropdownobj.AddOptions(options);
                    dropdownobj.value = 0;
                    dropdownobj.RefreshShownValue();
                }
                catch { }

                if (isInMainMenu)
                {
                    if (!string.IsNullOrWhiteSpace(lobbyIDCheckURL))
                    {
                        try
                        {
                            GameObject ui = GameObject.Find("UI").transform.GetChild(2).GetChild(1).gameObject;
                            GameObject bigbuttonPrefab = ui.transform.GetChild(0).gameObject;
                            GameObject spacerPrefab = ui.transform.GetChild(2).gameObject;

                            GameObject itemmodspacer = UnityEngine.Object.Instantiate<GameObject>(spacerPrefab);
                            itemmodspacer.name = "ItemMOD-spacer";
                            itemmodspacer.transform.SetParent(ui.transform);
                            itemmodspacer.transform.SetAsFirstSibling();

                            GameObject itemmodbutton = UnityEngine.Object.Instantiate<GameObject>(bigbuttonPrefab);
                            UnityEngine.Object.Destroy(itemmodbutton.GetComponent<UnityEngine.UI.Button>()); // Destroy that pesky create lobby function
                            itemmodbutton.name = "ItemMOD-247tag";
                            itemmodbutton.transform.SetParent(ui.transform);
                            itemmodbutton.transform.SetAsFirstSibling();
                            itemmodbutton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "24/7 TAG";
                            itemmodbutton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 72;
                            itemmodbutton.transform.localScale = Vector3.one;
                        }
                        catch { }
                    }

                    if (hideAbout) try { GameObject.Find("UI").transform.GetChild(2).GetChild(1).FindChild("About").gameObject.SetActive(false); } catch { }

                    try
                    {
                        createbutton(GameObject.Find("UI").transform.GetChild(6).GetChild(0).GetChild(1).GetChild(0).gameObject, GameObject.Find("UI").transform.GetChild(6).GetChild(0).GetChild(1).gameObject, "ItemMOD-clipjoin", "Join from clipboard", new Vector3(355, 0, 0));
                        GameObject.Find("UI").transform.GetChild(6).GetChild(0).GetChild(1).GetChild(5).gameObject.SetActive(false);
                    } catch { }
                }

                try
                {
                    GameObject toggleButtonParent = GameObject.Find("UI").transform.GetChild(4).GetChild(0).GetChild(2).gameObject;
                    if (textPrefab != null)
                    {
                        createtext(textPrefab, toggleButtonParent, "ItemMOD-boxrates-text", $"Box rates:", 20, new Vector3(280, 1, 0));
                        //text.transform.SetAsFirstSibling(); // Otherwise can make buttons unclickable - Currently done in createtext function
                    }
                    GameObject toggle = creattogglebutton(togglePrefab, toggleButtonParent, "ItemMOD-boxrates", new Vector3(360, 0, 0), showBoxRates);
                }
                catch { }

                GameObject settingsGameplay;
                if (isInMainMenu) settingsGameplay = GameObject.Find("UI").transform.GetChild(12).GetChild(1).GetChild(0).GetChild(0).gameObject;
                else settingsGameplay = GameObject.Find("GameUI").transform.GetChild(10).GetChild(0).GetChild(4).GetChild(1).GetChild(0).GetChild(0).gameObject;

                if (settingsGameplay != null)
                {
                    if (!string.IsNullOrWhiteSpace(WebsiteURL))
                    {
                        createbutton(buttonPrefab, settingsGameplay, "ItemMOD-Website", "ItemMOD Website", Vector3.one);
                    }
                    if (!string.IsNullOrWhiteSpace(DiscordURL))
                    {
                        createbutton(buttonPrefab, settingsGameplay, "ItemMOD-Discord", "ItemMOD Discord", Vector3.one);
                    }

                    createbutton(buttonPrefab, settingsGameplay, "ItemMOD-Config", "ItemMOD Config", Vector3.one);
                    createbutton(buttonPrefab, settingsGameplay, "ItemMOD-ReloadConf", "Reload ItemMOD Config", Vector3.one);
                    createbutton(buttonPrefab, settingsGameplay, "ItemMOD-LogOutput", "BepInEx LogOutput (advanced)", Vector3.one);
                    createbutton(buttonPrefab, settingsGameplay, "ItemMOD-CrabDir", "Crabgame folder", Vector3.one);
                    createbutton(buttonPrefab, settingsGameplay, "ItemMOD-PluginsDir", "Plugins folder", Vector3.one);

                    GameObject FOVSlider = settingsGameplay.transform.GetChild(1).GetChild(1).GetChild(0).gameObject;
                    UnityEngine.UI.Slider slider = FOVSlider.GetComponent<UnityEngine.UI.Slider>();
                    slider.maxValue = 180;
                    slider.m_MaxValue = 180;
                    slider.minValue = 0;
                    slider.m_MinValue = 0;
                }

                if (!isInMainMenu && showSpectatorCount)
                {
                    GameObject parent;
                    if (spectatorCountBelowCrosshair) parent = GameObject.Find("GameUI/Crosshair");
                    else parent = GameObject.Find("GameUI/Status/TopRight");
                    
                    GameObject text = createtext(textPrefab, parent, "ItemMOD-spectators", "", 16, Vector3.zero);
                    text.transform.SetAsLastSibling();
                    if (spectatorCountBelowCrosshair) text.transform.localPosition = new Vector3(0, -50, 0);
                    spectatorsText = text.GetComponent<TextMeshProUGUI>();
                }

                
            }

            yield return new WaitForSeconds(0.1f); // just need a return
        }

        [HarmonyPatch(typeof(TMP_Dropdown), nameof(TMP_Dropdown.OnSelectItem))]
        [HarmonyPrefix]
        public static void OnDropDownSelect(Toggle toggle)
        {
            switch (toggle.transform.name)
            {
                case "Item 0: Complete daily":
                    Quests.Instance.CompleteQuest();
                    SaveManager.Instance.Save();
                    break;
                case "Item 1: Claim daily":
                    SteamInventory.TryDailyQuest();
                    Prompt.Instance.NewPrompt("Done.", "Keep in mind steam limits you to 1 daily per 24 hours.");
                    break;
                case "Item 2: Get box":
                    SteamInventory.TryTriggerDrop();
                    Prompt.Instance.NewPrompt("Done.", "Keep in mind steam has a 24 hours per box limit (max 3 boxes).");
                    break;
                case "Item 3: Get box x3":
                    SteamInventory.TryTriggerDrop();
                    SteamInventory.TryTriggerDrop();
                    SteamInventory.TryTriggerDrop();
                    Prompt.Instance.NewPrompt("Done.", "Keep in mind steam has a 24 hours per box limit (max 3 boxes).");
                    break;
                case "Item 4: Do all":
                    SteamInventory.TryDailyQuest();
                    SteamInventory.TryTriggerDrop();
                    SteamInventory.TryTriggerDrop();
                    SteamInventory.TryTriggerDrop();
                    Prompt.Instance.NewPrompt("Done.", "Keep in mind steam has a 24 hours per box limit (max 3 boxes).\nAnd 1 daily per 24 hours");
                    break;
            }
        }

        public static GameObject createbutton(GameObject prefab, GameObject parent, string name, string text, Vector3 location)
        {
            GameObject newObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
            UnityEngine.Object.Destroy(newObject.GetComponent<UnityEngine.UI.Button>()); // Destroy that pesky original function
            if (!isInMainMenu)
            {
                UnityEngine.Object.Destroy(newObject.GetComponent<MonoBehaviourPublicIPointerEnterHandlerIEventSystemHandlerIPointerClickHandlerUnique>()); // pesky double clicking action whilst ingame
            }
            newObject.name = name;
            newObject.transform.SetParent(parent.transform);
            newObject.transform.localPosition = location;
            newObject.transform.FindChild("Text (TMP)").GetComponent<TextMeshProUGUI>().text = text;
            newObject.transform.localScale = Vector3.one;

            newObject.GetComponent<RectTransform>().sizeDelta = new Vector2(151.9f, 55.0f);

            return newObject;
        }

        public static GameObject createtext(GameObject prefab, GameObject parent, string name, string text, int fontsize, Vector3 location)
        {
            GameObject newObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
            //UnityEngine.Object.Destroy(newObject.GetComponent<UnityEngine.UI.Button>()); // Destroy that pesky original function
            newObject.name = name;
            newObject.transform.SetParent(parent.transform);
            newObject.transform.localPosition = location;
            newObject.transform.SetAsFirstSibling();
            newObject.GetComponent<TextMeshProUGUI>().text = text;
            newObject.GetComponent<TextMeshProUGUI>().fontSize = fontsize;
            newObject.GetComponent<TextMeshProUGUI>().fontSizeMax = fontsize;
            newObject.GetComponent<TextMeshProUGUI>().fontSizeMin = fontsize;
            newObject.transform.localScale = Vector3.one;

            return newObject;
        }

        public static GameObject creattogglebutton(GameObject prefab, GameObject parent, string name, Vector3 location, bool ischecked)
        {
            GameObject newObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
            UnityEngine.Object.Destroy(newObject.GetComponent<UnityEngine.UI.Button>()); // Destroy that pesky original function
            newObject.name = name;
            newObject.transform.SetParent(parent.transform);
            newObject.transform.localPosition = location;
            newObject.transform.localScale = Vector3.one;

            newObject.transform.FindChild("Toggle").gameObject.active = ischecked;

            return newObject;
        }

        [HarmonyPatch(typeof(ClientHandle), nameof(ClientHandle.SpectatingWho))]
        [HarmonyPrefix]
        public static bool OnSpectatingWho(Packet param_0)
        {
            if (!spectatorsInTAB && !showSpectatorCount)
            {
                return true; // just run the original
            }

            ulong who = param_0.Method_Public_UInt64_Boolean_0();
            ulong what = param_0.Method_Public_UInt64_Boolean_0();

            if (!GameManager.Instance.activePlayers.ContainsKey(what))
            {
                return false;
            }

            spectatorsList[who] = what;

            if (spectatorsList[who] == clientId)
            {
                spectatingME.Add(who);
            }
            else
            {
                spectatingME.Remove(who);
            }

            if (showSpectatorCount)
            {
                if (spectatingME.Count == 0)
                {
                    spectatorsText.text = "";
                }
                else
                {
                    spectatorsText.text = "Spectators: " + spectatingME.Count;
                } 
            }

            //doTabListStuff();

            PlayerManager playermanager = GameManager.Instance.activePlayers[what];
            if (GameManager.Instance.activePlayers.ContainsKey(who))
            {
                GameManager.Instance.activePlayers[who].vcTransform.target = playermanager.transform;
            }
            if (GameManager.Instance.spectators.ContainsKey(who))
            {
                GameManager.Instance.spectators[who].vcTransform.target = playermanager.transform;
            }

            return false;
        }

        [HarmonyPatch(typeof(PlayerList), nameof(PlayerList.UpdateList))]
        [HarmonyFinalizer]
        public static void UpdateList(PlayerList __instance)
        {
            doTabListStuff();
        }

        public static void doTabListStuff()
        {
            if (!spectatorsInTAB) return;

            GameObject tabList = GameObject.Find("GameUI/PlayerList/WindowUI/Tab0/Container/Content");
            Transform[] children = tabList.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child == tabList.transform)
                {
                    continue;
                }

                try
                {
                    MonoBehaviourPublicRabaicRaTeusscTepiObUnique playerValues = child.GetComponent<MonoBehaviourPublicRabaicRaTeusscTepiObUnique>();

                    //if (playerValues.field_Private_UInt64_0 == 123) // Sick mod
                    //{
                    //    child.FindChild("Name").GetComponent<TextMeshProUGUI>().color = new Color(1, (float)0.75, 0, 1);
                    //}

                    if (spectatorsList.ContainsKey(playerValues.field_Private_UInt64_0))
                    {
                        child.FindChild("Name").GetComponent<TextMeshProUGUI>().text = $"{GetPlayerSteamName(playerValues.field_Private_UInt64_0)} => {GetPlayerSteamName(spectatorsList[playerValues.field_Private_UInt64_0])}";
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Instance.PlayerDied))]
        [HarmonyPrefix]
        public static void PlayerDied(ulong param_1, ulong param_2, Vector3 param_3)
        {
            if (localdeathmessages || hostdeathmessages)
            {
                string message = "";
                switch (param_2)
                {
                    case 0:
                        message = $"{GetPlayerSteamName(param_1)}[{GetPlayerNumber(param_1)}] Died.";
                        break;
                    case 1:
                        message = $"{GetPlayerSteamName(param_1)}[{GetPlayerNumber(param_1)}] Exploded.";
                        break;
                    default:
                        message = $"{GetPlayerSteamName(param_2)}[{GetPlayerNumber(param_2)}] Killed {GetPlayerSteamName(param_1)}[{GetPlayerNumber(param_1)}]";
                        break;
                }

                if (SteamManager.Instance.IsLobbyOwner() && hostdeathmessages)
                {
                    SendServerMessage(message);
                }
                else
                {
                    ForceMessageModWhite(message);
                }
            }
        }

        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.Instance.OnPlayerJoinLeaveUpdate))]
        [HarmonyPrefix]
        public static bool OnPlayerJoinLeave(CSteamID param_1, bool param_2)
        {
            if (SteamManager.Instance.IsLobbyOwner()) ServerSend.LobbySettingsUpdate(LobbyManager.Instance.gameSettings, (ulong)param_1);

            if (GameManager.Instance)
            {
                if (SteamManager.Instance.IsLobbyOwner())
                {
                    string str = "joined";
                    if (!param_2)
                    {
                        str = "left";
                    }
                    string message = GetPlayerSteamName(param_1) + " " + str + " the server";
                    if (hostjoinleavemessages)
                    {
                        SendServerMessage(message);
                    }
                    else
                    {
                        ChatBox.Instance.AppendMessage(1, message, "");
                    }
                }
                else
                {
                    if (localjoinleavemessages)
                    {
                        string str = "joined";
                        if (!param_2)
                        {
                            str = "left";
                        }
                        string message = GetPlayerSteamName(param_1) + " " + str + " the server";
                        ForceMessageModOrange(message);
                    }
                }
            }
            return false;
        }

        public static void checkUpdate()
        {
            //ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
            string resultstring = "";
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    resultstring = webClient.DownloadString($"https://raw.githubusercontent.com/W-i-n-7/ItemMOD/refs/heads/main/update.txt");
                }

                string[] lines = resultstring.Split('\n');

                foreach (string line in lines)
                {
                    string[] parts = line.Split(";;;");
                    if (parts[0].StartsWithMod("update") && parts.Length >= 2 && parts.Length <= 4)
                    {
                        int newapiver = int.TryParse(parts[1], out int result) ? result : -1;
                        if (newapiver > apiver)
                        {
                            if (parts.Length >= 3) Prompt.Instance.NewPrompt("ItemMOD", "New ItemMOD version is available.\n\n" + parts[2]);
                            else Prompt.Instance.NewPrompt("ItemMOD", "New ItemMOD version is available.");

                            if (parts.Length == 4 && parts[3].StartsWithMod("http"))
                            {
                                UpdateURL = parts[3];

                                try
                                {
                                    createbutton(GameObject.Find("UI").transform.GetChild(3).GetChild(0).GetChild(5).GetChild(0).GetChild(0).gameObject, GameObject.Find("UI"), "ItemMOD-update", "Update ItemMOD", new Vector3(-885, -512, 1));
                                }
                                catch { }
                            }
                        }
                    }

                    if (parts[0].StartsWithMod("lobbyid") && parts.Length == 2)
                    {
                        lobbyIDCheckURL = parts[1];
                    }

                    if (parts[0].StartsWithMod("discord-link") && parts.Length == 2)
                    {
                        DiscordURL = parts[1];
                    }
                    if (parts[0].StartsWithMod("website-link") && parts.Length == 2)
                    {
                        WebsiteURL = parts[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Update check failed: " + ex.Message);
                Prompt.Instance.NewPrompt("ItemMOD", "Update check failed.");
            }
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Start))]
        [HarmonyPostfix]
        public static void OnSteamManagerStart(SteamManager __instance)
        {
            clientId = (ulong)__instance.field_Private_CSteamID_0;

            checkUpdate();

            if (showBoxRates)
            {
                GameObject ratesdisplay = GameObject.Find("UI").transform.GetChild(18).gameObject;
                ratesdisplay.active = true; 
            }

            createbuttonsez();
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Update))]
        [HarmonyPostfix]
        public static void OnSteamManagerUpdate(SteamManager __instance)
        {
            if (PlayerInventory.Instance != null && PlayerInventory.Instance.currentItem != null && totalReloadTime > 0f)
            {
                currentReloadTime += Time.deltaTime;
                int gamestate = (LobbyManager.Instance != null) ? (int)LobbyManager.Instance.state : 0;
                if (gamestate != 0 && gamestate != 1)
                {
                    float num = currentReloadTime / totalReloadTime;
                    PlayerInventory.Instance.currentItem.transform.localRotation = Quaternion.Euler(-EaseInOutBack(num) * (360f * Mathf.CeilToInt(totalReloadTime)), 0f, 0f);
                    if (num >= 1f)
                    {
                        totalReloadTime = 0f;
                        currentReloadTime = 0f;
                    }
                }
            }
        }

        private static float EaseInOutBack(float x)
        {
            float num = 2.5949094f;
            bool flag = x < 0.5f;
            float result;
            if (flag)
            {
                result = Mathf.Pow(2f * x, 2f) * ((num + 1f) * 2f * x - num) / 2f;
            }
            else
            {
                result = (Mathf.Pow(2f * x - 2f, 2f) * ((num + 1f) * (x * 2f - 2f) + num) + 2f) / 2f;
            }
            return result;
        }

        [HarmonyPatch(typeof(GameUI), "Awake")]
        [HarmonyPostfix]
        public static void UIAwakePatch(GameUI __instance)
        {
            GameObject menuObject = new GameObject();
            Main basics = menuObject.AddComponent<Main>();

            menuObject.transform.SetParent(__instance.transform);
        }

        //Anticheat Bypass 
        [HarmonyPatch(typeof(EffectManager), "Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0")]
        [HarmonyPatch(typeof(LobbyManager), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(MonoBehaviourPublicVesnUnique), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(LobbySettings), "Method_Public_Void_PDM_2")]
        [HarmonyPatch(typeof(MonoBehaviourPublicTeplUnique), "Method_Private_Void_PDM_32")]
        [HarmonyPrefix]
        public static bool Prefix(System.Reflection.MethodBase __originalMethod)
        {
            return false;
        }
    }

    public static class StringExtensions
    {
        public static bool StartsWithMod(this string str, string prefix)
        {
            return str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
    }

    //public static class GameObjectExtensions
    //{
    //    public static string GetFullPath(this GameObject obj)
    //    {
    //        List<string> path = new List<string>();
    //        Transform current = obj.transform;

    //        while (current != null)
    //        {
    //            path.Add(current.name);
    //            current = current.parent;
    //        }

    //        path.Reverse();
    //        return string.Join("/", path.ToArray());
    //    }
    //}
}