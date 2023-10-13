using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Numerics;
using GrindLife;
using Pc;
using Raylib_cs;

Raylib.SetTargetFPS(60);
Raylib.InitWindow(1400, 900, "The Grind Life");


string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
string filePath = Path.Combine(path, "worlds.txt");
Worlds level = new();
bool jumping = false;
bool showInv = false;
bool isMap = false;
Rectangle createButton = new Rectangle(250, 500, 200, 75);
Rectangle loadButton = new();
Vector2 mousePos = new Vector2();
string sceene = "start";
Random gen = new();
bool inLevel = false;
string newName = "";
int currentLevel = 0;
load();
while (!Raylib.WindowShouldClose())
{

    mousePos = Raylib.GetMousePosition();
    Raylib.BeginDrawing();
    if (!inLevel)
    {
        if (sceene == "start")
        {
            Raylib.ClearBackground(Color.BLUE);
            Raylib.DrawText("Load level or Create New", 450, 30, 50, Color.BLACK);
            Raylib.DrawRectangleRec(createButton, Color.GRAY);
            Raylib.DrawRectangleLinesEx(createButton, 2, Color.BLACK);
            Raylib.DrawText("Create new World", 255, 505, 20, Color.BLACK);
            if (Raylib.CheckCollisionPointRec(mousePos, createButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    sceene = "newWorld";
                }
            }
            if (level.worldList.Count > 0)
            {
                int recPosx = 650;
                int recPosy = 500;
                List<Rectangle> loadList = new();
                for (int i = 0; i < level.worldList.Count; i++)
                {
                    loadButton = new Rectangle(recPosx, recPosy, 200, 75);
                    loadList.Add(loadButton);
                    recPosx += 300;
                }
                recPosx = 650;
                for (int i = 0; i < loadList.Count; i++)
                {
                    Raylib.DrawRectangleRec(loadList[i], Color.GRAY);
                    Raylib.DrawRectangleLinesEx(loadList[i], 2, Color.BLACK);
                    Raylib.DrawText(level.worldList[i].name, recPosx+5, recPosy+5, 24, Color.BLACK);
                    if (Raylib.CheckCollisionPointRec(mousePos, loadList[i]))
                    {
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            sceene = $"{level.worldList[i].name}";
                            inLevel = true;
                        }
                    }
                    recPosx += 300;
                }
            }
        }

        if (sceene == "newWorld")
        {
            Raylib.ClearBackground(Color.GREEN);
            Raylib.DrawText("Name your world", 500, 30, 40, Color.BLACK);
            Rectangle createNewButton = new Rectangle(550, 700, 300, 100);
            int key = Raylib.GetKeyPressed();
            if (key != 0)
            {
                if (key == 13)
                {
                    createNewWorld();
                }
                if (key == 259)
                {
                    try
                    {
                        newName = newName.Substring(0, newName.Length - 1);
                    }
                    catch
                    {
                        newName = newName.Substring(0, newName.Length);
                    }
                }
                else if (newName.Length < 12)
                {
                    newName += (char)key;
                }
            }
            Raylib.DrawText(newName, 400, 500, 24, Color.BLACK);
            Raylib.DrawRectangleRec(createNewButton, Color.GRAY);
            Raylib.DrawText("Create", 555, 705, 24, Color.BLACK);
            if (Raylib.CheckCollisionPointRec(mousePos, createNewButton))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    createNewWorld();
                }
            }
        }
    }

    if (inLevel)
    {
        for (int i = 0; i < level.worldList.Count; i++)
        {
            if (sceene == level.worldList[i].name)
            {
                currentLevel = i;
            }
        }

        if (level.worldList[currentLevel].screen == "start")
        {
            Raylib.ClearBackground(Color.RED);
            Raylib.DrawRectangle((int)level.worldList[currentLevel].character.location.X, (int)level.worldList[currentLevel].character.location.Y, 50, 50, Color.BLUE);
            putInHand();
            showInventory();
            move();
            showMap();

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_J))
            {
                level.worldList[currentLevel].character.inventory.Add(new Items { name = "Pickaxe", description = "Tool to mine", hitPower = 15 });
            }
            Vector2 hotPos = new Vector2(20, 50);
            for (int i = 0; i < level.worldList[currentLevel].character.inventory.Count; i++)
            {
                Raylib.DrawText($"{i + 1}. {level.worldList[currentLevel].character.inventory[i].name}", (int)hotPos.X, (int)hotPos.Y, 20, Color.BLACK);
                hotPos.Y += 25;
            }
            if (level.worldList[currentLevel].character.inventory.Count > 9)
            {
                level.worldList[currentLevel].character.hiddenInventory.Add(level.worldList[currentLevel].character.inventory[8]);
                level.worldList[currentLevel].character.inventory.RemoveAt(8);
            }
            save();
        }
        if (level.worldList[currentLevel].screen == "mine")
        {
            Raylib.ClearBackground(Color.DARKGRAY);
            Raylib.DrawRectangle((int)level.worldList[currentLevel].character.location.X, (int)level.worldList[currentLevel].character.location.Y, 50, 50, Color.BLUE);
            putInHand();
            showInventory();
            move();
            showMap();
            Raylib.DrawText("Press 'P' to mine!", 500, 40, 30, Color.WHITE);
            Raylib.DrawText($"${level.worldList[currentLevel].character.money}", 1250, 50, 24, Color.WHITE);
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
            {
                level.worldList[currentLevel].character.money += gen.Next(level.worldList[currentLevel].character.hand.hitPower + 5);
            }
            Vector2 hotPos = new Vector2(20, 50);
            for (int i = 0; i < level.worldList[currentLevel].character.inventory.Count; i++)
            {
                Raylib.DrawText($"{i + 1}. {level.worldList[currentLevel].character.inventory[i].name}", (int)hotPos.X, (int)hotPos.Y, 20, Color.BLACK);
                hotPos.Y += 25;
            }
            save();
        }

        save();
    }


    Raylib.EndDrawing();
}






//-------------------------------------------- Voids --------------------------------------------------------------------------------






void putInHand()
{
    int key = Raylib.GetKeyPressed();
    if (key > 48 && key < 58 && key - 49 < level.worldList[currentLevel].character.inventory.Count)
    {
        level.worldList[currentLevel].character.hand = level.worldList[currentLevel].character.inventory[key - 49];
    }
}

void showMap()
{
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_M))
    {
        if (isMap)
        {
            isMap = false;
        }
        else
        {
            isMap = true;
        }
    }
    if (isMap)
    {
        Rectangle mineButton = new Rectangle(500, 650, 250, 100);
        Rectangle startButton = new Rectangle(850, 650, 250, 100);
        Rectangle shopButton = new Rectangle(500, 500, 250, 100);
        Raylib.DrawRectangle(50, 50, 1300, 800, Color.WHITE);
        Raylib.DrawRectangleRec(mineButton, Color.GRAY);
        Raylib.DrawText("Mine", 505, 655, 24, Color.BLACK);
        Raylib.DrawRectangleRec(startButton, Color.GRAY);
        Raylib.DrawText("Start", 855, 655, 24, Color.BLACK);
        Raylib.DrawRectangleRec(shopButton, Color.GRAY);
        Raylib.DrawText("Store", 505, 505, 24, Color.BLACK);
        if (Raylib.CheckCollisionPointRec(mousePos, mineButton))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                level.worldList[currentLevel].screen = "mine";
                isMap = false;
            }
        }
        if (Raylib.CheckCollisionPointRec(mousePos, startButton))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                level.worldList[currentLevel].screen = "start";
                isMap = false;
            }
        }
        if (Raylib.CheckCollisionPointRec(mousePos, shopButton))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                level.worldList[currentLevel].screen = "store";
                isMap = false;
            }
        }
    }
}

void showInventory()
{
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_I))
    {
        if (!showInv)
        {
            showInv = true;
        }
        else
        {
            showInv = false;
        }
    }
    if (showInv)
    {
        Vector2 textPos = new Vector2(1250, 50);
        for (int i = 0; i < level.worldList[currentLevel].character.hiddenInventory.Count; i++)
        {
            Raylib.DrawText(level.worldList[currentLevel].character.hiddenInventory[i].name, (int)textPos.X, (int)textPos.Y, 24, Color.BLACK);
            textPos.Y += 40;
        }
    }
}



void createNewWorld()
{
    level.worldList.Add(new worldInfo { name = newName, seed = gen.Next(1000), character = new Player { name = $"Character{level.worldList.Count}" } });
    inLevel = true;
    sceene = $"{newName}";
    save();
}


void move()
{
    float fallingSpeed = 2;
    if (level.worldList[currentLevel].character.location.Y < 800)
    {
        fallingSpeed = (level.worldList[currentLevel].character.location.Y - 600)/100;
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
    {
        level.worldList[currentLevel].character.location.X += 2;
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
    {
        level.worldList[currentLevel].character.location.X -= 2;
    }
    if (level.worldList[currentLevel].character.location.Y < 800 && !jumping)
    {
        level.worldList[currentLevel].character.location.Y += fallingSpeed;
    }
    if (level.worldList[currentLevel].character.location.Y > 800)
    {
        level.worldList[currentLevel].character.location.Y = 800;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) && level.worldList[currentLevel].character.location.Y == 800)
    {
        jumping = true;
    }
    if (jumping)
    {
        level.worldList[currentLevel].character.location.Y -= fallingSpeed;
        if (level.worldList[currentLevel].character.location.Y <= 700)
        {
            jumping = false;
        }
    }
}



void save()
{
    var jsonSetting = new Newtonsoft.Json.JsonSerializerSettings
    {
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
    };
    var serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(level, Newtonsoft.Json.Formatting.Indented, jsonSetting);
    using (StreamWriter sw = new StreamWriter(filePath))
    {
        sw.Write(serializedObject);
    }
}

void load()
{
    var jsonSetting = new Newtonsoft.Json.JsonSerializerSettings
    {
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
    };
    string content = null;
    using (StreamReader sr = new StreamReader(filePath))
    {
        content = sr.ReadToEnd();
    }
    var loadedInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Worlds>(content, jsonSetting);
    level = loadedInfo;
}