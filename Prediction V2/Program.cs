﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using SharpDX;
using Ensage.Common.Extensions;


namespace Prediction_V2
{
    class Program
    {
        public static Hero me;
        public static bool inGame = false;
        public static int EnemyIndex = 0;
        public static Tracker[] EnemyTracker = { new Tracker(null, 0), new Tracker(null, 0), new Tracker(null, 0), new Tracker(null, 0), new Tracker(null, 0), };

        static void Main(string[] args)
        {
            Drawing.OnDraw += Drawing_OnDraw; //Graphical Drawer
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            #region Fundamentals
            me = ObjectMgr.LocalHero;
            if (!inGame)
            {
                if (!Game.IsInGame || me == null)
                    return;
                inGame = true;
                Success("Indera keenam menyala");

            }
            if (!Game.IsInGame || me == null)
            {
                inGame = false;
                return;
            }
            #endregion

            var players = ObjectMgr.GetEntities<Player>().ToList();
            if (!players.Any())
                return;
            EnemyIndex = 0;
            int enemyIndex = 0;
            foreach (var enemy in EnemyHeroNotIllusion(players))
            {
                if (EnemyTracker[enemyIndex].EnemyTracker != null) //Draw last known direction
                    LastKnownPosition(enemy, enemyIndex);
                enemyIndex++;
            }
        }

        public static void LastKnownPosition(Hero enemy, int enemyIndex)
        {
            try
            {
                if (enemy.IsAlive)
                {
                    var Angle = enemy.FindAngleR();
                    Vector2 StraightDis = Drawing.WorldToScreen(enemy.Position); //Facing position line
                    StraightDis.X += (float)Math.Cos(Angle) * 500;
                    StraightDis.Y += (float)Math.Sin(Angle) * 500;
                    if (Drawing.WorldToScreen(EnemyTracker[enemyIndex].EnemyTracker.Position).Y > 15)
                    {
                        Drawing.DrawLine(Drawing.WorldToScreen(EnemyTracker[enemyIndex].EnemyTracker.Position), StraightDis, Color.Red);
                        Drawing.DrawText(string.Format("{0} {1}", GetHeroNameFromLongHeroName(enemy.Name), GetTimeDifference(EnemyTracker[enemyIndex].RelativeGameTime)), Drawing.WorldToScreen(EnemyTracker[enemyIndex].EnemyTracker.Position), Color.Cyan, FontFlags.AntiAlias | FontFlags.Outline);
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        public static List<Hero> EnemyHeroNotIllusion(List<Player> baseList)
        {
            try
            {
                return baseList.Where(player => player.Hero.Team != me.Team && !player.Hero.IsIllusion).Select(player => player.Hero).ToList();
            }
            catch { return new List<Hero>(); }
        }

        public static string GetHeroNameFromLongHeroName(string Name)
        {
            return Name.Split(new string[] { "npc_dota_hero_" }, StringSplitOptions.None)[1];
        }
        public static string GetTimeDifference(int Time)
        {
            int difference = (int)Game.GameTime - Time;
            if (difference == 0)
                return "";
            else if (difference < 2)
                return difference.ToString() + " detik";
            else if (difference < 60)
                return difference.ToString() + " detik";
            else
                return ConvertIntToTimeString(difference) + " ago";
        }
        public static string ConvertIntToTimeString(int Time)
        {
            TimeSpan result = TimeSpan.FromSeconds(Time);
            return result.ToString("mm':'ss");
        }

        public static void Success(string text, params object[] arguments)
        {
            Encolored(text, ConsoleColor.Green, arguments);
        }
        public static void Encolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
    }

    public class Tracker
    {
        public Hero EnemyTracker { get; set; }
        public int RelativeGameTime { get; set; }
        public Tracker(Hero target, int time)
        {
            EnemyTracker = target;
            RelativeGameTime = time;
        }
    }
}
