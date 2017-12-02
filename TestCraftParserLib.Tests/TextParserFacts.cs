using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCraftParserLib.Tests
{
    [TestClass]
    public class TextParserFacts
    {
        public TextParserFacts()
        {
            Parser = new TextParser();
        }

        private TextParser Parser { get; }

        [TestMethod]
        public void Can_get_first_line()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:29:11] [Server] You are now in a Full PVP Area.
[CHAT WINDOW TEXT] [Sun Dec 03 00:29:22] [Server] You are now in a Full PVP Area.";

            var results = Parser.PreParse(text).ToList();
            Assert.AreEqual(2, results.Count);
            string expected1 = "[CHAT WINDOW TEXT] [Sun Dec 03 00:29:11] [Server] You are now in a Full PVP Area.";
            Assert.AreEqual(expected1, results[0]);
        }

        [TestMethod]
        public void Can_get_second_line()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:29:11] [Server] You are now in a Full PVP Area.
[CHAT WINDOW TEXT] [Sun Dec 03 00:29:22] [Server] You are now in a Full PVP Area.";

            var results = Parser.PreParse(text).ToList();
            Assert.AreEqual(2, results.Count);
            string expected1 = "[CHAT WINDOW TEXT] [Sun Dec 03 00:29:22] [Server] You are now in a Full PVP Area.";
            Assert.AreEqual(expected1, results[1]);
        }

        [TestMethod]
        public void Can_get_text_on_multi_lines()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:41:37] Weapon Crafting Anvil - Standard: Craftable Natural Resources CNR - Drow Wars Modified. Based on CNR V3.05
Automated Batch Restriction of 35 Craftable Items per craft. Exp Restrictions, Crafting Levels 1-4 = 250xp max, Levels 5-9 = 500xp max, Levels 10-14 = 750xp max, Levels 15-19 = 1000xp max 

What would you like to make?
Weapon Crafting Anvil - Standard: [Talk] <c�>Craftable Natural Resources <czz�>CNR - Drow Wars Modified. Based on CNR V3.05
Automated Batch Restriction of 35 Craftable Items per craft. Exp Restrictions, Crafting Levels 1-4 = 250xp max, Levels 5-9 = 500xp max, Levels 10-14 = 750xp max, Levels 15-19 = 1000xp max </c></c>

What would you like to make? 
[CHAT WINDOW TEXT] [Sun Dec 03 00:41:38] Lianna: Copper Weapons";

            var results = Parser.PreParse(text).ToList();
            Assert.AreEqual(2, results.Count);
            string expected = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:41:37] Weapon Crafting Anvil - Standard: Craftable Natural Resources CNR - Drow Wars Modified. Based on CNR V3.05
Automated Batch Restriction of 35 Craftable Items per craft. Exp Restrictions, Crafting Levels 1-4 = 250xp max, Levels 5-9 = 500xp max, Levels 10-14 = 750xp max, Levels 15-19 = 1000xp max 

What would you like to make?
Weapon Crafting Anvil - Standard: [Talk] <c�>Craftable Natural Resources <czz�>CNR - Drow Wars Modified. Based on CNR V3.05
Automated Batch Restriction of 35 Craftable Items per craft. Exp Restrictions, Crafting Levels 1-4 = 250xp max, Levels 5-9 = 500xp max, Levels 10-14 = 750xp max, Levels 15-19 = 1000xp max </c></c>

What would you like to make? ";
            Assert.AreEqual(expected, results[0]);
        }

        [TestMethod]
        public void Test_can_find_craft_location()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:41:44] Weapon Crafting Anvil - Standard: 
Copper Dwarven Waraxe

Components available/required...
---------------------------

0 of 4   Ingot of Copper
0 of 1   Small Casting Mold
0 of 1   Shaft of Hickory";

            var recipe = Parser.GetRecipe(text);
            Assert.AreEqual("Weapon Crafting Anvil - Standard", recipe.Location);
        }

        [TestMethod]
        public void Can_get_requirement_amount()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:41:44] Weapon Crafting Anvil - Standard: 
Copper Dwarven Waraxe

Components available/required...
---------------------------

0 of 4   Ingot of Copper
0 of 1   Small Casting Mold
0 of 1   Shaft of Hickory";

            var recipe = Parser.GetRecipe(text);
            Assert.AreEqual(4, recipe.Requirements.First().Amount);
        }

        [TestMethod]
        public void Can_get_requirement_name()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:41:44] Weapon Crafting Anvil - Standard: 
Copper Dwarven Waraxe

Components available/required...
---------------------------

0 of 4   Ingot of Copper
0 of 1   Small Casting Mold
0 of 1   Shaft of Hickory";

            var recipe = Parser.GetRecipe(text);
            Assert.AreEqual("Ingot of Copper", recipe.Requirements.First().ItemName);
        }

        [TestMethod]
        public void Can_get_a_recipe()
        {
            string text = @"[CHAT WINDOW TEXT] [Sun Dec 03 00:41:44] Weapon Crafting Anvil - Standard: 
Copper Dwarven Waraxe

Components available/required...
---------------------------

0 of 4   Ingot of Copper
0 of 1   Small Casting Mold
0 of 1   Shaft of Hickory

Given your strength and dexterity, this recipe is trivial for you to make.
Weapon Crafting Anvil - Standard: [Talk] 
Copper Dwarven Waraxe

Components available/required...
---------------------------

0 of 4   Ingot of Copper
0 of 1   Small Casting Mold
0 of 1   Shaft of Hickory

Given your strength and dexterity, this recipe is <c�>trivial</c> for you to make. 
";

            var results = Parser.PreParse(text).ToList();
            var recipes = results.Select(x => Parser.GetRecipe(x)).Where(x => x != null).ToList();

            Assert.AreEqual(1, recipes.Count);
        }
    }
}
