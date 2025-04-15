using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace SpartaDungeon
{
    internal class Program
    {

        static void ShowStatus(string characterName, int lv, string jobName, int hp, int attack, int defense, int gold)

        {
            Console.Clear();
            Console.WriteLine("===========================");
            Console.WriteLine($"| 이름   : {characterName}");
            Console.WriteLine($"| 레벨   : {lv}");
            Console.WriteLine($"| 직업   : {jobName}");
            Console.WriteLine($"| 체력   : {hp}");
            Console.WriteLine($"| 공격력 : {attack}");
            Console.WriteLine($"| 방어력 : {defense}");
            Console.WriteLine($"| Gold   : {gold}");
            Console.WriteLine("===========================");
            Console.WriteLine("엔터를 누르면 마을 화면으로 갑니다.");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            string characterName = "";
            string jobName = "";
            int lv = 0;
            int hp = 0;
            int attack = 0;
            int defense = 0;
            int gold = 0;

            Console.WriteLine("안녕하세요. 스파르타 던전에 오신것을 환영 합니다.");
            Console.WriteLine("당신의 이름은 무엇 입니까?: ");
            characterName = Console.ReadLine();

            Console.WriteLine($"환영합니다, {characterName} 님!");
            

            int job;
            bool validJob = false;

            while (!validJob)
            {
                Console.WriteLine("1. 전사 2. 궁수 3. 도적");
                Console.WriteLine("위의 직업 중에서 당신의 직업을 선택 해 주세요.: ");
                string selectClass = Console.ReadLine();

                if (int.TryParse(selectClass, out job)) // 선택한 번호를 상수로 바꿔준다
                {
                    switch (job) //선택한 번호에 따라 직업의 정보를 상태 보기에 저장 한다.
                    {
                        case 1:
                            lv = 1;
                            jobName = "전사";
                            hp = 100;
                            attack = 10;
                            defense = 5;
                            gold = 1500;
                            validJob = true;
                            break;
                        case 2:
                            lv = 1;
                            jobName = "궁수";
                            hp = 80;
                            attack = 15;
                            defense = 3;
                            gold = 1500;
                            validJob = true;
                            break;
                        case 3:
                            lv = 1;
                            jobName = "도적";
                            hp = 90;
                            attack = 12;
                            defense = 3;
                            gold = 2000;
                            validJob = true;
                            break;
                        default:
                            Console.WriteLine("올바른 숫자를 입력해 주세요 (1, 2, 3).");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력해 주세요 (1, 2, 3).");
                }
            }
            int menu;

            while (validJob)
            {
                Console.Clear();   // 이전 화면 지우기

                // 마을 화면 출력
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
                Console.WriteLine("1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("0. 게임 종료");
                Console.Write("원하시는 행동을 입력해주세요.: ");
                string selectmenu = Console.ReadLine();

                if(int.TryParse(selectmenu, out menu))
                {
                   switch(menu)
                    {
                        case 1:
                            ShowStatus(characterName, lv, jobName, hp, attack, defense, gold);
                            break;
                        case 2:
                            Console.WriteLine("인벤토리를 아직 구현하지 않았습니다.");
                            Console.ReadLine();
                            break;
                        case 3:
                            Console.WriteLine("상점을 아직 구현하지 않았습니다.");
                            Console.ReadLine();
                            break;
                        case 0:
                            Console.WriteLine("게임을 종료합니다.");
                            return;
                        default:
                            Console.WriteLine("올바른 숫자를 입력해 주세요.");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력해 주세요.");
                    Console.ReadLine();
                }
            }
        }
    }
}