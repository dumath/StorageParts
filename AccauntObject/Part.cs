using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AccauntObject
{
    public class Part
    {
        #region Fields
        //TODO:порядковый номер : для всех объектов(собственность класса)
        private static int _number = 0;     //Общее количество созданных объектов.
        private string _brand;              //Бренд
        private string _name;               //Класс запчасти - наименование.
        private string _originalNumber;     //Номер по оригиналу 
        private string _analogNumber;       //Номер по аналогу
        private int _count;                 //Количество
        private readonly decimal _buyPrice; //Цена покупки !!Нельзя изменить 

        //TODO:Проверка - _sellPrice всегда должна быть больше закупочной
        private decimal _sellPrice;         //Цена продажи 
        private string _firstComment;       //Первый комментарий ?? для чего первый
        private string _secondComment;      //Второй комментарий ?? для чего второй
        #endregion

        #region Constructors
        //Пассивный конструктор
        public Part() { }

        //Активный конструктор: Для полной инициализации объекта.
        public Part(string brand, string name, string originalNumber, string analogNumber, int count, decimal buyPrice, decimal sellPrice, string firstComment, string secondCcomment)
        {
            _number += 1;
            this._brand = brand;
            this._name = name;
            this._originalNumber = originalNumber;
            this._analogNumber = analogNumber;
            this._count = count;
            this._buyPrice = buyPrice;
            this._sellPrice = sellPrice;
            this._firstComment = firstComment;
            this._secondComment = secondCcomment;
        }

        //Активный конструктор: Для инициализации объекта из базы данных.
        public Part(string stringDb)
        {
            string brand = stringDb.Substring(stringDb.IndexOf("Brand:"), stringDb.IndexOf("; Name"));
            brand = brand.Substring(6);
            this._brand = brand;

            string name = stringDb.Substring(stringDb.IndexOf("Name:"));
            name = name.Substring(name.IndexOf("Name:"), name.IndexOf("; ON:"));
            name = name.Substring(5);
            this._name = name;

            string on = stringDb.Substring(stringDb.IndexOf("ON:"));
            on = on.Substring(on.IndexOf("ON:"), on.IndexOf("; AN:"));
            on = on.Substring(3);
            this._originalNumber = on;

            string an = stringDb.Substring(stringDb.IndexOf("AN:"));
            an = an.Substring(an.IndexOf("AN:"), an.IndexOf("; Count"));
            an = an.Substring(3);
            this._analogNumber = an;

            string count = stringDb.Substring(stringDb.IndexOf("Count:"));
            count = count.Substring(count.IndexOf("Count:"), count.IndexOf("; BuyPrice"));
            count = count.Substring(6);
            this._count = Int32.Parse(count);

            string bp = stringDb.Substring(stringDb.IndexOf("BuyPrice:"));
            bp = bp.Substring(bp.IndexOf("BuyPrice:"), bp.IndexOf("; SellPrice:"));
            bp = bp.Substring(9);
            this._buyPrice = decimal.Parse(bp);

            string sp = stringDb.Substring(stringDb.IndexOf("SellPrice:"));
            sp = sp.Substring(sp.IndexOf("SellPrice:"), sp.IndexOf("; FC:"));
            sp = sp.Substring(10);
            this._sellPrice = decimal.Parse(sp);

            string fc = stringDb.Substring(stringDb.IndexOf("FC:"));
            fc = fc.Substring(fc.IndexOf("FC:"), fc.IndexOf("; SC:"));
            fc = fc.Substring(3);
            this._firstComment = fc;

            string sc = stringDb.Substring(stringDb.IndexOf("SC:"));
            sc = sc.Substring(3);
            this._secondComment = sc;
            _number += 1;
        }

        //Активный конструктор: Для инициализации через инициализатор объекта.
        public Part(decimal buyPrice)
        {
            _number += 1;
            this._buyPrice = buyPrice;
        }
        #endregion

        #region Properties
        //Количество созданных объектов.
        public static int Number
        {
            get => _number;
        }

        //Бренд запчасти.
        public string Brand
        {
            get => this._brand;
            set
            {
                this._brand = value;
            }
        }

        //Имя запчасти(наименование).
        public string Name
        {
            get => this._name;
            set
            {
                this._name = value;
            }
        }

        //Номер по оригиналу.
        public string OriginalNumber
        {
            get => this._originalNumber;
            set
            {
                this._originalNumber = value;
            }
        }

        //Номер по аналогу.
        public string AnalogNumber
        {
            get => this._analogNumber;
            set
            {
                this._analogNumber = value;
            }
        }

        //Количество.
        public int Count
        {
            get => this._count;
            set
            {
                this._count = value;
            }
        }

        //Покупка.
        public decimal BuyPrice
        {
            get => this._buyPrice;
        }

        //Продажа.
        public decimal SellPrice
        {
            get => this._sellPrice;
            set
            {
                this._sellPrice = value;
            }
        }

        //Первый комментарий.
        public string FirstComment
        {
            get => this._firstComment;
            set
            {
                this._firstComment = value;
            }
        }

        //Второй комментарий.
        public string SecondComment
        {
            get => this._secondComment;
            set
            {
                this._secondComment = value;
            }
        }
        #endregion

        #region Methods
        //Перегруженный метод объекта: строковое прдеставление, для записи в файл.
        //TODO: StringBuilder??
        public override string ToString()
        {
            return $"Brand:{this._brand}; Name:{this._name}; ON:{this._originalNumber}; AN:{this._analogNumber}; Count:{this._count}; BuyPrice:{this._buyPrice}; SellPrice:{this._sellPrice}; FC:{this._firstComment}; SC:{this._secondComment}";
        }

        //TODO: Реализовать сравнение Hash
        //Метод сравнения двух Part объектов.
        public override bool Equals(object obj)
        {
            Part p = obj as Part;
            if(p != null)
            {
                if (this._brand != p.Brand)
                    return false;
                if (this._name != p.Name)
                    return false;
                if (this._originalNumber != p.OriginalNumber)
                    return false;
                if (this._analogNumber != p.AnalogNumber)
                    return false;
                if (this._count != p.Count)
                    return false;
                if (this._originalNumber != p.OriginalNumber)
                    return false;
                if (this._analogNumber != p.AnalogNumber)
                    return false;
                if(this._buyPrice != p.BuyPrice)
                    return false;
                if(this._sellPrice != p.SellPrice)
                    return false;
                if(this._firstComment != p.FirstComment)
                    return false;
                if(this._secondComment != p.SecondComment)
                    return false;
                return true;
            }
            return false;
        }
        #endregion
    }
}
