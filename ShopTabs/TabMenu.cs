﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopTabs
{
    internal class TabMenu : IClickableMenu
    {
        public ShopMenu targetMenu;

        public Dictionary<ISalable, ItemStockInformation> shopItems;

        public List<ClickableTextureComponent> filterTabs = new();

        public static List<string> filterTypes = new()
        {
            "Seeds", // type == Seeds, category == -74
            "Saplings", // type == Basic, catagory == -74
            "Cooking", // type == Cooking, catagory == -7
            "Fertillizer", // type == Basic, catagory == -19
            "Crops", // type == Basic, catagory == -75
            "Recipes",
            "Other"
        };

        public TabMenu(ShopMenu menu, Dictionary<ISalable, ItemStockInformation> shopItems)
        {
            this.targetMenu = menu;
            menu.SetChildMenu(this);
            this.shopItems = shopItems;

            if (shopItems != null)
            {
                for (int i = 0; i < filterTypes.Count; i++)
                {
                    string assetName = "assets/" + filterTypes[i];
                    ClickableTextureComponent tab = new(
                            filterTypes[i], new Rectangle(menu.xPositionOnScreen + 64 * i, menu.yPositionOnScreen - 60, 64, 64), "",
                            filterTypes[i], ModEntry.Content.Load<Texture2D>(assetName), new Rectangle(0, 0, 16, 16), 4f);
                    filterTabs.Add(tab);
                }
            }
        }

        public void ApplyTab(string type)
        {
            Dictionary<ISalable, ItemStockInformation> newStock = new();

            foreach (var item in this.shopItems)
            {
                if (item.Key is StardewValley.Object obj && MatchItemType(obj, type))
                {
                    newStock[item.Key] = item.Value;
                }
            }

            targetMenu.itemPriceAndStock = newStock;
            targetMenu.forSale = newStock.Keys.ToList();
        }

        public static bool MatchItemType(StardewValley.Object obj, string type)
        {
            int[] categories = { -74, -7, -19, -75 }; // update this list if new types are added
            switch (type)
            {
                case "Seeds":
                    return obj.Type == "Seeds" && obj.Category == -74;
                case "Saplings":
                    return obj.Type == "Basic" && obj.Category == -74;
                case "Cooking":
                    return obj.Type == "Cooking" && obj.Category == -7;
                case "Fertillizer":
                    return obj.Type == "Basic" && obj.Category == -19;
                case "Crops":
                    return obj.Type == "Basic" && obj.Category == -75;
                case "Recipes":
                    return obj.IsRecipe;
                case "Other":
                    return !(categories.Contains(obj.Category) || obj.IsRecipe);
                default:
                    return false;
            };
        }

        public override void draw(SpriteBatch b)
        {
            targetMenu.draw(b);
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                tab.draw(b);
            }
            drawMouse(b);
        }

        public override void performHoverAction(int x, int y)
        {
            targetMenu.performHoverAction(x, y);
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                tab.tryHover(x, y);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableTextureComponent tab in filterTabs)
            {
                if (tab.containsPoint(x, y))
                {
                    ApplyTab(tab.name);
                }
            }
            targetMenu.receiveLeftClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            targetMenu.receiveScrollWheelAction(direction);
        }

        public override void update(GameTime time)
        {
            targetMenu.update(time);
        }

        public override bool readyToClose()
        {
            // Close both TabMenu and ShopMenu when targetMenu is ready to close
            if (targetMenu.readyToClose())
            {
                Game1.activeClickableMenu = null;
                return true;
            }
            return false;
        }

        public override void snapToDefaultClickableComponent()
        {
            targetMenu.snapToDefaultClickableComponent();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            targetMenu.snapCursorToCurrentSnappedComponent();
        }
    }
}