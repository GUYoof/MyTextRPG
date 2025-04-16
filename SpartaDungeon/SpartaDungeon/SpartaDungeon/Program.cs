using System.Diagnostics;
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
        public Item(string name, string description, int price, ItemType type)
        {
            Name = name;
            Price = price;
            Description = description;
            Type = type;
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

        public void ShowStatus()
        {
            Console.Clear();
            Console.WriteLine("===========================");
            Console.WriteLine($"| 이름   : {Name}");
            Console.WriteLine($"| 레벨   : {Level}");
            Console.WriteLine($"| 직업   : {Job}");
            Console.WriteLine($"| 체력   : {HP}");
            Console.WriteLine($"| 공격력 : {Attack}");
            Console.WriteLine($"| 방어력 : {Defense}");
            Console.WriteLine($"| Gold   : {Gold}");
            Console.WriteLine("===========================");
            Console.WriteLine("엔터를 누르면 마을 화면으로 갑니다.");
            Console.ReadLine();
        }

        public void ShowInventory()
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
                Console.WriteLine($"- {item.Name} : {item.Description} (가격: {item.Price})");
            }

            Console.WriteLine("\n1. 장착하기");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("\n원하는 행동을 입력해주세요.");
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
                new Item("수련자 갑옷", "방어력 +5 | 수련에 도움을 주는 갑옷입니다.", 500, ItemType.Armor),
                new Item("무쇠 갑옷", "방어력 +9 | 무쇠로 만들어져 튼튼한 갑옷입니다.", 800, ItemType.Armor),
                new Item("스파르타의 갑옷", "방어력 +15 | 전설의 갑옷입니다.", 1500, ItemType.Armor),
                new Item("낡은 검", "공격력 +2 | 쉽게 볼 수 있는 낡은 검입니다.", 300, ItemType.Weapon),
                new Item("청동 도끼", "공격력 +5 | 어딘가 사용된 흔적이 있는 도끼입니다.", 700, ItemType.Weapon),
                new Item("스파르타의 창", "공격력 +7 | 전설의 전사들이 사용한 창입니다.", 1200, ItemType.Weapon)
            };
        }

        public void Enter(Player player)
        {
            while (true)
            {
                ShowItemList(player);
                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("\n원하는 행동을 입력하세요.");
                Console.Write("\n선택: ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    return; // 상점 종료
                }
                else if (input == "1")
                {
                    ProceedPurchase(player);
                } 
                else 
                    {
                        Console.WriteLine("올바른 번호를 입력해주세요."); Console.ReadLine(); 
                    }
            }
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
                Console.WriteLine($"- {item.Name} : {item.Description} ({item.Price} G)");
            }
        }

        private void ProceedPurchase(Player player)
        {
            Console.Clear();
            Console.WriteLine("구매할 아이템 번호를 선택하세요.\n");

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                Console.WriteLine($"{i + 1}. {item.Name} - {item.Description} ({item.Price} G)");
            }
            Console.WriteLine("\n어떤 아이템을 구매하겠습니까? 번호를 입력하세요.");
            Console.WriteLine("\n0. 취소하고 돌아가기");
            Console.Write("\n선택: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int index))
            {
                if (index == 0) 
                    return;
                if (index >= 1 && index <= Items.Count)
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
                player.Inventory.Add(selectedItem);
                Console.WriteLine($"'{selectedItem.Name}'을(를) 구매했습니다!");
            }
            else Console.WriteLine("Gold가 부족합니다.");
            Console.ReadLine();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("안녕하세요. 스파르타 던전에 오신것을 환영합니다.");
            Console.Write("당신의 이름은 무엇입니까?: ");
            string characterName = Console.ReadLine();

            string jobName = "";
            int lv = 1, hp = 0, attack = 0, defense = 0, gold = 0;
            bool validJob = false;

            while (!validJob)
            {
                Console.WriteLine("1. 전사 2. 궁수 3. 도적");
                Console.Write("당신의 직업을 선택해주세요: ");
                string selectClass = Console.ReadLine();

                if (int.TryParse(selectClass, out int job))
                {
                    switch (job)
                    {
                        case 1: jobName = "전사"; hp = 100; attack = 10; defense = 5; gold = 1500; validJob = true; 
                            break;
                        case 2: jobName = "궁수"; hp = 80; attack = 15; defense = 3; gold = 1500; validJob = true; 
                            break;
                        case 3: jobName = "도적"; hp = 90; attack = 12; defense = 3; gold = 2000; validJob = true; 
                            break;
                        default: Console.WriteLine("올바른 숫자를 입력하세요."); 
                            break;
                    }
                }
                else Console.WriteLine("숫자를 입력하세요.");
            }

            Player player = new Player(characterName, jobName, lv, hp, attack, defense, gold);
            Shop shop = new Shop();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("스파르타 마을에 오신 것을 환영합니다.");
                Console.WriteLine("1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("0. 게임 종료");
                Console.Write(">> ");

                string select = Console.ReadLine();
                if (int.TryParse(select, out int menu))
                {
                    switch (menu)
                    {
                        case 1: player.ShowStatus(); 
                            break;
                        case 2: player.ShowInventory(); 
                            break;
                        case 3: shop.Enter(player); 
                            break;
                        case 0: Console.WriteLine("게임을 종료합니다."); 
                            return;
                        default: Console.WriteLine("올바른 숫자를 입력해주세요."); 
                            Console.ReadLine(); 
                            break;
                    }
                }
                else { Console.WriteLine("숫자를 입력해주세요."); 
                    Console.ReadLine(); }
            }
        }
    }
}