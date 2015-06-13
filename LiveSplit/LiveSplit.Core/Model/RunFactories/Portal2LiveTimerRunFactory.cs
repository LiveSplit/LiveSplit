using LiveSplit.Model.Comparisons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.RunFactories
{
    public class Portal2LiveTimerRunFactory : IRunFactory
    {
        public Stream Stream { get; set; }

        public Portal2LiveTimerRunFactory(Stream stream = null)
        {
            Stream = stream;
        }

        public IRun Create(IComparisonGeneratorsFactory factory)
        {
            var chapters = new Dictionary<string, string[]>()
            {
                { 
                    "Chapter 1 - The Courtesy Call",
                    new []
                    {
                        "sp_a1_intro1",
                        "sp_a1_intro2",
                        "sp_a1_intro3",
                        "sp_a1_intro4",
                        "sp_a1_intro5",
                        "sp_a1_intro6",
                        "sp_a1_intro7",
                        "sp_a1_wakeup",
                        "sp_a2_intro"
                    }
                },
                {
                    "Chapter 2 - The Cold Boot",
                    new []
                    {
                        "sp_a2_laser_intro",
                        "sp_a2_laser_stairs",
                        "sp_a2_dual_lasers",
                        "sp_a2_laser_over_goo",
                        "sp_a2_catapult_intro",
                        "sp_a2_trust_fling",
                        "sp_a2_pit_flings",
                        "sp_a2_fizzler_intro"
                    }
                },
                {
                    "Chapter 3 - The Return",
                    new []
                    {
                        "sp_a2_sphere_peek",
                        "sp_a2_ricochet",
                        "sp_a2_bridge_intro",
                        "sp_a2_bridge_the_gap",
                        "sp_a2_turret_intro",
                        "sp_a2_laser_relays",
                        "sp_a2_turret_blocker",
                        "sp_a2_laser_vs_turret",
                        "sp_a2_pull_the_rug",
                    }
                },
                {
                    "Chapter 4 - The Surprise",
                    new []
                    {
                        "sp_a2_column_blocker",
                        "sp_a2_laser_chaining",
                        "sp_a2_triple_laser",
                        "sp_a2_bts1",
                        "sp_a2_bts2",
                    }
                },
                {
                    "Chapter 5 - The Escape",
                    new []
                    {
                        "sp_a2_bts3",
                        "sp_a2_bts4",
                        "sp_a2_bts5",
                        "sp_a2_bts6",
                        "sp_a2_core",
                    }
                },
                {
                    "Chapter 6 - The Fall",
                    new []
                    {
                        "sp_a3_00",
                        "sp_a3_01",
                        "sp_a3_03",
                        "sp_a3_jump_intro",
                        "sp_a3_bomb_flings",
                        "sp_a3_crazy_box",
                        "sp_a3_transition01",
                    }
                },
                {
                    "Chapter 7 - The Reunion",
                    new []
                    {
                        "sp_a3_speed_ramp",
                        "sp_a3_speed_flings",
                        "sp_a3_portal_intro",
                        "sp_a3_end",
                    }
                },
                {
                    "Chapter 8 - The Itch",
                    new []
                    {
                        "sp_a4_intro",
                        "sp_a4_tb_intro",
                        "sp_a4_tb_trust_drop",
                        "sp_a4_tb_wall_button",
                        "sp_a4_tb_polarity",
                        "sp_a4_tb_catch",
                        "sp_a4_stop_the_box",
                        "sp_a4_laser_catapult",
                        "sp_a4_laser_platform",
                        "sp_a4_speed_tb_catch",
                        "sp_a4_jump_polarity",
                    }
                },
                {
                    "Chapter 9 - The Part Where...",
                    new []
                    {
                        "sp_a4_finale1",
                        "sp_a4_finale2",
                        "sp_a4_finale3",
                        "sp_a4_finale4",
                        //"sp_a5_credits",
                    }
                }
            };

            var run = new Run(factory);

            run.GameName = "Portal 2";
            run.CategoryName = "Any%";

            var reader = new StreamReader(Stream);

            var lines = reader
                .ReadToEnd()
                .Split('\n')
                .Select(x => x.Replace("\r", ""))
                .Skip(1)
                .Select(x => x.Split(','))
                .ToArray();

            var aggregateTicks = 0;

            foreach (var chapter in chapters)
            {
                foreach (var map in chapter.Value)
                {
                    //Force it to break, if the splits aren't there
                    lines.First(x => x[0] == map); 

                    foreach (var mapLine in lines.Where(x => x[0] == map))
                    {
                        var mapTicks = int.Parse(mapLine[2], CultureInfo.InvariantCulture)
                                      - int.Parse(mapLine[1], CultureInfo.InvariantCulture);
                        aggregateTicks += mapTicks;
                    }
                }

                var timeSpan = TimeSpan.FromSeconds(aggregateTicks / 60.0);
                var chapterTime = new Time(timeSpan, timeSpan);

                run.AddSegment(chapter.Key, chapterTime);
            }

            return run;
        }
    }
}
