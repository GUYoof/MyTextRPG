using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using SpartaDungeon;

namespace SpartaDungeon
{
    public enum ItemType { Armor, Weapon }

    class Item
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public bool IsSold { get; set; } = false;
        public bool IsEquipped = false;

        public Item(string name, string description, int price, ItemType type, int attact = 0, int defense = 0)
        {
            Name = name;
            Price = price;
            Description = description;
            Type = type;
            Attack = attact;
            Defense = defense;
        }
    }

    class Player
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Gold { get; set; }

        public List<Item> Inventory { get; private set; } = new List<Item>();

        public Player(string name, string job, int level, int hp, int attack, int defense, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            HP = hp;
            Attack = attack;
            Defense = defense;
            Gold = gold;
        }

        public int GetTotalAttack()
        {
            int bonus = Inventory.Where(i => i.Type == ItemType.Weapon).Sum(i => i.Attack);
            return Attack + bonus;
        }

        public int GetTotalDefense()
        {
            int bonus = Inventory.Where(i => i.Type == ItemType.Armor).Sum(i => i.Defense);
            return Defense + bonus;
        }

        public void ShowStatus()
        {
            int bonusAttack = 0;
            int bonusDefense = 0;

            foreach (var item in Inventory)
            {
                bonusAttack += item.Attack;
                bonusDefense += item.Defense;
            }

            Console.Clear();
            Console.WriteLine("===========================");
            Console.WriteLine($"| 이름   : {Name}");
            Console.WriteLine($"| 레벨   : {Level}");
            Console.WriteLine($"| 직업   : {Job}");
            Console.WriteLine($"| 체력   : {HP}");

            // 보너스가 있는 경우에만 (+숫자) 출력
            string attackText = bonusAttack > 0 ? $"{Attack} +({bonusAttack})" : $"{Attack}";
            string defenseText = bonusDefense > 0 ? $"{Defense} +({bonusDefense})" : $"{Defense}";

            Console.WriteLine($"| 공격력 : {attackText}");
            Console.WriteLine($"| 방어력 : {defenseText}");
            Console.WriteLine($"| Gold   : {Gold}");
            Console.WriteLine("===========================");
            Console.WriteLine("엔터를 누르면 마을 화면으로 갑니다.");
            Console.ReadLine();
        }

        public void ShowInventory()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("인벤토리");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine("\n[아이템 목록]");

                if (Inventory.Count == 0)
                {
                    Console.WriteLine("보유 중인 아이템이 없습니다.");
                }
                else
                {
                    foreach (var item in Inventory)
                    {
                        string equippedMark = item.IsEquipped ? "[E] " : "";
                        string statText = "";

                        if (item.Type == ItemType.Weapon && item.Attack > 0)
                            statText = $"공격력 +{item.Attack}";
                        else if (item.Type == ItemType.Armor && item.Defense > 0)
                            statText = $"방어력 +{item.Defense}";

                        Console.WriteLine($"- {equippedMark}{item.Name} {statText} {item.Description} ({item.Type})");
                    }
                }

                Console.WriteLine("\n1. 장착하기");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("\n원하는 행동을 입력해주세요.");
                string select = Console.ReadLine();

                if (int.TryParse(select, out int Difficulty))
                    if (select == "0")
                    {
                        return; // 인벤토리 종료
                    }
                    else if (select == "1")
                    {
                        ProcessedEquipment();
                    }
                    else
                    {
                        Console.WriteLine("올바른 번호를 입력해주세요."); Console.ReadLine();
                    }
                else
                {
                    Console.WriteLine("숫자를 입력하세요.");
                    Console.ReadLine();
                }
            }
        }

        public void ProcessedEquipment()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("인벤토리 - 장착 관리");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine("\n[아이템 목록]");

                if (Inventory.Count == 0)
                {
                    Console.WriteLine("보유 중인 아이템이 없습니다.");
                }
                else
                {
                    for (int i = 0; i < Inventory.Count; i++)
                    {
                        var item = Inventory[i];
                        string equippedMark = item.IsEquipped ? "[E] " : "";

                        string statText = "";
                        if (item.Type == ItemType.Weapon && item.Attack > 0)
                            statText = $"공격력 +{item.Attack}";
                        else if (item.Type == ItemType.Armor && item.Defense > 0)
                            statText = $"방어력 +{item.Defense}";

                        Console.WriteLine($"{i + 1}. {equippedMark}{item.Name} {statText} {item.Description} ({item.Type})");
                    }
                }
                Console.WriteLine("\n장착하거나 해제할 아이템을 선택해 주세요.");
                Console.WriteLine("\n0. 취소하고 돌아가기");
                Console.Write("\n선택: ");
                string select = Console.ReadLine();

                if (int.TryParse(select, out int index))
                {
                    if (index == 0)
                        return;
                    else if (index >= 1 && index <= Inventory.Count)
                    {
                        EquippedItem(index);
                    }
                    else
                    {
                        Console.WriteLine("올바른 번호를 입력해주세요.");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Console.ReadLine();
                }
            }
        }

        public void EquippedItem(int index)
        {
            var selectedItem = Inventory[index - 1];

            if (!selectedItem.IsEquipped)
            {
                foreach (var item in Inventory)
                    if (item.Type == selectedItem.Type && item.IsEquipped)
                    {
                        item.IsEquipped = false;
                        Console.WriteLine($"{item.Name}이 장착 해제되었습니다.");
                    }
                selectedItem.IsEquipped = true;
                Console.WriteLine($"\n{selectedItem.Name}을(를) 장착했습니다.");
            }
            else
            {
                selectedItem.IsEquipped = false;
                Console.WriteLine($"\n{selectedItem.Name}을(를) 해제했습니다.");
            }

            Console.WriteLine("\n엔터를 누르면 돌아갑니다.");
            Console.ReadLine();
        }
    }

    class Shop
    {
        public List<Item> Items { get; private set; }

        public Shop()
        {
            Items = new List<Item>()
                {
                    new Item("티셔츠", "| 얇은 천옷입니다.", 500, ItemType.Armor, 0, 2),
                    new Item("수련자 갑옷", "| 수련에 도움을 주는 갑옷입니다.", 1000, ItemType.Armor, 0, 5),
                    new Item("무쇠 갑옷", "| 무쇠로 만들어져 튼튼한 갑옷입니다.", 1800, ItemType.Armor, 0, 9),
                    new Item("스파르타의 갑옷", "| 전설의 갑옷입니다.", 3500, ItemType.Armor, 0, 15),
                    new Item("몽둥이", "| 나무로 만들어진 몽둥이입니다.", 300, ItemType.Weapon, 1, 0),
                    new Item("낡은 검", "| 쉽게 볼 수 있는 낡은 검입니다.", 600, ItemType.Weapon, 2, 0),
                    new Item("청동 도끼", "| 어딘가 사용된 흔적이 있는 도끼입니다.", 1500, ItemType.Weapon, 5, 0),
                    new Item("스파르타의 창", "| 전설의 전사들이 사용한 창입니다.", 2700, ItemType.Weapon, 7, 0)
                };
        }

        private void ShowItemList(Player player)
        {
            Console.Clear();
            Console.WriteLine("상점");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G\n");

            Console.WriteLine("[아이템 목록]");
            foreach (var item in Items)
            {
                string priceText = item.IsSold ? "판매 완료" : $"{item.Price} G";

                string statText = "";
                if (item.Type == ItemType.Weapon && item.Attack > 0)
                {
                    statText = $"공격력 +{item.Attack}";
                }
                else if (item.Type == ItemType.Armor && item.Defense > 0)
                {
                    statText = $"방어력 +{item.Defense}";
                }
                Console.WriteLine($"- {item.Name} : {statText} {item.Description} ({priceText})");
            }
        }

        public void ShopEnter(Player player)
        {
            while (true)
            {
                ShowItemList(player);
                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("\n원하는 행동을 입력하세요.");
                Console.Write("\n선택: ");
                string select = Console.ReadLine();

                if (int.TryParse(select, out int Difficulty))
                {
                    if (Difficulty == 0)
                        return;
                    else if (Difficulty >= 1 && Difficulty <= Items.Count)
                    {
                        ProceedPurchase(player);
                    }
                    else
                    {
                        Console.WriteLine("올바른 번호를 입력해주세요.");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력해주세요."); 
                    Console.ReadLine();
                }
            }
        }

        private void ProceedPurchase(Player player)
        {
            Console.Clear();
            Console.WriteLine("구매할 아이템 번호를 선택하세요.\n");

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                string priceText = item.IsSold ? "판매 완료" : $"{item.Price} G";

                string statText = "";
                if (item.Type == ItemType.Weapon && item.Attack > 0)
                {
                    statText = $"공격력 +{item.Attack}";
                }
                else if (item.Type == ItemType.Armor && item.Defense > 0)
                {
                    statText = $"방어력 +{item.Defense}";
                }
                Console.WriteLine($"{i + 1}.  {item.Name} : {statText} {item.Description} ({priceText})");
            }
            Console.WriteLine("\n어떤 아이템을 구매하겠습니까? 번호를 입력하세요.");
            Console.WriteLine("\n0. 취소하고 돌아가기");
            Console.Write("\n선택: ");
            string select = Console.ReadLine();

            if (int.TryParse(select, out int index))
            {
                if (index == 0)
                    return;
                else if (index >= 1 && index <= Items.Count)
                {
                    HandlePurchase(player, index);
                }
                else
                {
                    Console.WriteLine("올바른 번호를 입력해주세요.");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
                Console.ReadLine();
            }
        }

        private void HandlePurchase(Player player, int index)
        {
            var selectedItem = Items[index - 1];
            if (player.Inventory.Any(i => i.Name == selectedItem.Name))
                Console.WriteLine("이미 구매한 아이템입니다.");
            else if (player.Gold >= selectedItem.Price)
            {
                player.Gold -= selectedItem.Price;
                selectedItem.IsSold = true;
                player.Inventory.Add(selectedItem);
                Console.WriteLine($"'{selectedItem.Name}'을(를) 구매했습니다!");
            }
            else Console.WriteLine("Gold가 부족합니다.");
            Console.ReadLine();
        }
    }

    class Dungeon
    {
        public void DungeonEnter(Player player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("던전 입장");
                Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
                Console.WriteLine("\n1. 쉬운 던전    | 방어력 5 이상 권장");
                Console.WriteLine("2. 일반 던전    | 방어력 11 이상 권장");
                Console.WriteLine("3. 어려운 던전  | 방어력 17 이상 권장");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write("\n>> ");
                string select = Console.ReadLine();

                if (int.TryParse(select, out int Difficulty)) 
                {
                    switch (Difficulty)
                    {
                        case 1:
                            EasyDungeon(player);
                            break;
                        case 2:
                            NormalDongeo(player);
                            break;
                        case 3:
                            HardDongeo(player);
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("올바른 숫자를 입력하세요.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력하세요.");
                    Console.ReadLine();
                }
            }
        }

        public void EasyDungeon(Player player)
        {
            Console.Clear();
            Console.WriteLine("쉬운 던전");
            Console.WriteLine("방어력 5 이상 권장하는 던전 입니다.");

            if (player.GetTotalDefense() >= 5)
            {
                DungeonClear(player, 5, 1000);
            }
            else
            {
                ConditionsLack(player, 5);
            }

            Console.WriteLine("\n엔터를 누르면 던전 선택으로 갑니다.");
            Console.ReadLine();
        }

        public void NormalDongeo(Player player)
        {
            Console.Clear();
            Console.WriteLine("보통 던전");
            Console.WriteLine("방어력 11 이상 권장하는 던전 입니다.");

            if (player.GetTotalDefense() >= 11)
            {
                DungeonClear(player, 11, 1000);
            }
            else
            {
                ConditionsLack(player, 11);
            }

            Console.WriteLine("\n엔터를 누르면 던전 선택으로 갑니다.");
            Console.ReadLine();
        }

        public void HardDongeo(Player player)
        {
            Console.Clear();
            Console.WriteLine("어려운 던전");
            Console.WriteLine("방어력 17 이상 권장하는 던전 입니다.");

            if (player.GetTotalDefense() >= 17)
            {
                DungeonClear(player, 17, 1000);
            }
            else
            {
                ConditionsLack(player, 17);
            }

            Console.WriteLine("\n엔터를 누르면 던전 선택으로 갑니다.");
            Console.ReadLine();
        }

        public void DungeonClear(Player player, int requiredDefense, int baseReward)
        {
            Console.WriteLine("\n던전 클리어");
            Console.WriteLine("축하합니다!!");
            Console.WriteLine("던전을 클리어 하였습니다.");
            Console.WriteLine("\n[탐험 결과]");

            int defenseCorrectionValue = player.GetTotalDefense() - requiredDefense;
            int beforeHp = player.HP;
            int beforeGold = player.Gold;

            // 기본 범위에서 방어력 보정 적용
            int minDamage = 20 - defenseCorrectionValue;
            int maxDamage = 35 - defenseCorrectionValue;

            Random damage = new Random();
            int playerDamage = damage.Next(minDamage, maxDamage + 1); // 범위 포함

            player.HP -= playerDamage;

            if (player.HP < 0) 
            {
                player.HP = 0;
            }

            // 기본 보상에서 보너스 보상 적용
            int minBonusPercent = player.Attack;
            int maxBonusPercent = player.Attack * 2;

            Random Percent = new Random();
            int bonusPercent = Percent.Next(minBonusPercent, maxBonusPercent + 1);

            int bonusGold = baseReward * bonusPercent / 100;
            int totalGold = baseReward + bonusGold;

            player.Gold += totalGold;

            Console.WriteLine($"체력 {beforeHp} → {player.HP}");
            Console.WriteLine($"Gold {beforeGold} G → {player.Gold} G");
        }

        public void ConditionsLack (Player player, int requiredDefense)
        {
            Console.WriteLine("\n방어력이 부족하여 던전 탐험이 실패 할 수 있습니다.");

            Random rand = new Random();
            int chance = rand.Next(0, 101);

            if (chance <= 40)
            {
                Console.WriteLine("\n던전 실패...");
                Console.WriteLine("방어력이 부족하여 몬스터에게 패했습니다.");
                Console.WriteLine("보상 없이 되돌아왔습니다.");

                int defenseCorrectionValue = player.GetTotalDefense() - requiredDefense;
                int beforeHp = player.HP;

                int damage = player.HP / 2; // 절반 데미지
                player.HP -= damage;
                if (player.HP < 0)
                {
                    player.HP = 0;
                }

                Console.WriteLine($"체력 {beforeHp} → {player.HP}");
            }
            else
            {
                Console.WriteLine("\n운좋게 던전을 클리어했습니다!");
                DungeonClear(player, 5, 1000); // 난이도별 값 전달
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("안녕하세요. 스파르타 던전에 오신것을 환영합니다.");
            Console.Write("\n당신의 이름은 무엇입니까?: ");
            string characterName = Console.ReadLine();

            string jobName = "";
            int lv = 1, hp = 0, attack = 0, defense = 0, gold = 0;
            bool validJob = false;

            while (!validJob)
            {
                Console.WriteLine("\n1. 전사");
                Console.WriteLine("2. 궁수");
                Console.WriteLine("3. 도적");
                Console.Write("\n당신의 직업을 선택해주세요: ");
                string select = Console.ReadLine();

                if (int.TryParse(select, out int job))
                {
                    switch (job)
                    {
                        case 1:
                            jobName = "전사";
                            hp = 100;
                            attack = 10;
                            defense = 5;
                            gold = 1500;
                            validJob = true;
                            break;
                        case 2:
                            jobName = "궁수";
                            hp = 80;
                            attack = 15;
                            defense = 3;
                            gold = 1500;
                            validJob = true;
                            break;
                        case 3:
                            jobName = "도적";
                            hp = 90;
                            attack = 12;
                            defense = 3;
                            gold = 2000;
                            validJob = true;
                            break;
                        default:
                            Console.WriteLine("올바른 숫자를 입력하세요.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력하세요.");
                    Console.ReadLine();
                }
            }

            Player player = new Player(characterName, jobName, lv, hp, attack, defense, gold);
            Shop shop = new Shop();
            Dungeon dungeon = new Dungeon();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("스파르타 마을에 오신 것을 환영합니다.");
                Console.WriteLine("\n1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("4. 던전 입장");
                Console.WriteLine("0. 게임 종료");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write("\n>> ");
                string select = Console.ReadLine();
                if (int.TryParse(select, out int menu))
                {
                    switch (menu)
                    {
                        case 1:
                            player.ShowStatus();
                            break;
                        case 2:
                            player.ShowInventory();
                            break;
                        case 3:
                            shop.ShopEnter(player);
                            break;
                        case 4:
                            dungeon.DungeonEnter(player);
                            break;
                        case 0:
                            Console.WriteLine("게임을 종료합니다.");
                            return;
                        default:
                            Console.WriteLine("올바른 숫자를 입력해주세요.");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Console.ReadLine();
                }
            }
        }
    }
}   