using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DoulbeNode
{
    class ListNode
    {
        public ListNode Prev; //Предыдущий элемент
        public ListNode Next; //Следующий элемент
        public ListNode Rand; //Произвольный элемент 
        public string Data;   //Значение
    }

    class ListRand
    {
        public ListNode Head;//Начальный элемент
        public ListNode Tail;//Последный элемент
        public int Count;    // Кол-во элементов

        public int IdNode(ListNode tur)//Поиск порядкового номера элемента
        {
            int i;//счетчик
            ListNode cur = Head;//текущий - начальный элемент
            for (i = 0; i < Count; i++, cur=cur.Next)// проходим по всем элементам
                if (cur == tur)//Нашли нужный элемента
                    return i;//выводим индекс
            return -1;//если нет 
        }

        public ListNode Node(int id)//Поиск элемента по порядковому номеру 
        {
            int i;//счетчик
            ListNode cur = Head;//текущий - начальный элемент
            for (i = 0; i < Count; i++, cur = cur.Next)// проходим по всем элементам
                if (i == id)//Совпадение?
                    return cur;//вывод соответвующего элемента
            return null;// иначе нет значения
        }

        public void Serialize(Stream s)//Сериализация
        {
            ListNode cur = Head;//Текущий - первый элемент

            using (StreamWriter sw = new StreamWriter(s))//Открываем файл и начинаем запись
            {
                //int i = 0;// id счетчик
                sw.WriteLine("<ListRand>");//Формируем файл
                //Данные ListRand
                sw.WriteLine(" <Head>{0}</Head>", ((Head == null) ? "null" : "0")); //Начальный элемент
                sw.WriteLine(" <Tail>{0}</Tail>", ((Tail == null) ? "null" : (Count-1).ToString() ));//Последный элемент
                sw.WriteLine(" <Count>{0}</Count>", Count); // Кол-во элементов

                for(int i=0; cur != null; cur = cur.Next, i++)
                {
                    sw.WriteLine(String.Format(@"   <ListNode id=""{0}"">", i));//Создаем элемент
                    
                    sw.WriteLine(String.Format("    <Rand>{0}</Rand>", cur.Rand == null ? "null" : (IdNode(cur.Rand)==-1 ? "null" : IdNode(cur.Rand).ToString() )
                                 ) );//Случайный элемент
                    sw.WriteLine(String.Format("    <Data>{0}</Data>", cur.Data)); //Значение элемента
                    sw.WriteLine(String.Format("   </ListNode>"));  //Закрываем элемент
                } 

                sw.WriteLine("</ListRand>");//Закрываем документ

            }
        }
        public void Deserialize(Stream s)//Десериализация
        {
            int t = 0;//Номер послед элемента 
            Dictionary<int, int> rand= new Dictionary<int, int>();//Словарь [номер элемента, порядковый номер произвольного элемента]
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(s);//Открываем файл
            XmlElement xRoot = xDoc.DocumentElement;//Открываем корневой элемент
            ListNode cur=null;//Текущий элемент
            foreach (XmlNode xnode in xRoot)//Проходим по элементам 
            {             
                if (xnode.Name == "Tail")
                    t = int.Parse(xnode.InnerText);//Номер последнего элемента
                if (xnode.Name == "Count")
                    Count = int.Parse(xnode.InnerText);//Число элементов
                if (xnode.Name == "ListNode")//создание элемента списка 
                {
                    
                    XmlNode attr = xnode.Attributes.GetNamedItem("id");//номер элемента
                    ListNode nnode= new ListNode()//Инициализация нового элемента
                    {
                         Prev = cur,//Указываем на предыдущий
                         Next = null,//Указываем на следующий элементы
                    };
                    if (cur != null) cur.Next = nnode;//Указание предыдущего элемента
                    foreach (XmlNode childnode in xnode.ChildNodes)//Проходим по характеристикам элемента
                    {
                        if (childnode.Name == "Data")//Значение
                            nnode.Data = childnode.InnerText;
                        if (childnode.Name == "Rand")//Номер произвольного элемента
                            rand.Add(int.Parse(attr.Value.ToString()), int.Parse(childnode.InnerText));//Добавляем в словарь
                    }

                    if (attr.Value.ToString()=="0")//Если номер эелемента 0
                        Head = nnode;//То объявляем начальный элемент
                    cur = nnode;//текущий элемент равен новому
                }
            }
            s.Close();//закрываем файл
            Tail = Node(t);//Находим конечный элемент
            cur = Head;//Текущий равен начальному
            for (int i = 0; cur!=null; i++, cur=cur.Next)
                cur.Rand = Node(rand[i]);//По словарю определяем произвольный элемент для текущего
            cur = Head;//Текущий равен начальному
            //Для проверки вывод
            //for (int i = 0; cur != null; i++, cur = cur.Next)
            //    Console.WriteLine("i="+i+" Data:"+ cur.Data.ToString() + " Rand:" + cur.Rand.Data.ToString());
            return;
        }
    }
        class Program
    {
        static Random rand = new Random();//Для случайного значения
   
        static ListNode AddNode(ListNode prev)//Добавление элемента в двусвязный список
        {
            ListNode element = new ListNode()//Инициализация нового элемента
            {
                Prev = prev,//Указываем на предыдущий
                Next = null,//Указываем на следующий элементы
                Data = rand.Next(0, 100).ToString(),//Случайное значние
            };
            if(prev!=null) prev.Next = element;//Предудущиему элементу указываем значение на следующего на новый
            return element;//Возврат нового элемента
        }
        static ListNode randomNode(ListNode head, int length)//Поискслучайного элемента
        {
            int k = rand.Next(0, length);//Случайное значение номера элемента
            ListNode cur = head; //Текущий элемент
            for (int i=0; i < k;i++)
                cur = cur.Next;//Находим элемнт
            return cur;//И возвращаем его
        }
        static void Main(string[] args)
        {
            //==Создание двусвязного списка==
            ListNode head = AddNode(null);//Первый элемент
            ListNode cur = head;//Текущий элемент
            int length = 7;//Колличество элементов в списке
            for (int i = 1; i < length; i++)
                cur = AddNode(cur);
            ListNode tail = cur;//Последний элемент
            cur = head;//Текущий снова первый элемент

            //Добавляем случайный элемент
            for (int i = 0; i < length; i++)
            {
                cur.Rand = randomNode(head, length);
                cur = cur.Next;
            }
            //==============================

            //==Реализация сериализации и десиреализации==
            //Обявление класса
            ListRand s = new ListRand();
            s.Head = head;//Первый 
            s.Tail = tail;//Последний элемент
            s.Count = length;//Колличество элементов

            //==Сереализация
            Stream fs = new FileStream("file.xml", FileMode.Create); ;
            s.Serialize(fs);
            //==Десериализация
            ListRand d = new ListRand();
            fs = new FileStream("file.xml", FileMode.Open);
            d.Deserialize(fs);
            //============================================
        }
    }
}
