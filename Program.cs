namespace GameTask
{
    public enum CharacterType { Warrior, Mage }

    public abstract class Character
    {
        public required string Name { get; set; }
        public int Strength { get; set; }
        public int Magic { get; set; }
        public int Health { get; private set; }
        public abstract CharacterType Type { get; }

        protected Character(int strength, int magic, int health)
        {
            Strength = strength;
            Magic = magic;
            Health = health;
        }

        public void TakeDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);
        }
    }

    public interface IWeapon
    {
        int CalculateDamage(Character attacker);
    }
   
    public class Warrior : Character
    {
        public Warrior() : base(5, 0, 100) { }
        public override CharacterType Type => CharacterType.Warrior;
    }

    public class Mage : Character
    {
        public Mage() : base(1, 2, 50) { }
        public override CharacterType Type => CharacterType.Mage;
    }

    public class Sword : IWeapon
    {
        public int CalculateDamage(Character attacker) => attacker.Strength * 3;
    }

    public class MagicStaff : IWeapon
    {
        public int CalculateDamage(Character attacker) => (attacker.Magic * 2) + 2;
    }

    public class FoolsWand : IWeapon
    {
        private readonly Random _random = new();

        public int CalculateDamage(Character attacker)
        {            
            if (attacker is Mage && _random.Next(2) == 0)
            { 
                // можно кортеж, но я старый:)
                int temp = attacker.Strength;
                attacker.Strength = attacker.Magic;
                attacker.Magic = temp;
            }

            return _random.Next(0, 11);
        }
    }
   
    public class GameEngine
    {
        /// <summary>
        /// Наносит урон противнику на основе выбранного оружия.
        /// API спроектировано так, чтобы можно было передать любого персонажа и любое оружие.
        /// </summary>
        public void InflictDamage(Character attacker, Character target, IWeapon weapon)
        {
            if (attacker == null || target == null || weapon == null)
                return;

            if (target.Health <= 0)
            {
                Console.WriteLine($"{target.Name} уже повержен.");
                return;
            }

            int damage = weapon.CalculateDamage(attacker);
            target.TakeDamage(damage);

            Console.WriteLine($"{attacker.Name} атакует {target.Name} используя {weapon.GetType().Name}!");
            Console.WriteLine($"Нанесено урона: {damage}. У {target.Name} осталось HP: {target.Health}");

            if (attacker is Mage mage)
            {
                Console.WriteLine($"Текущие статы мага: Сила {mage.Strength}, Магия {mage.Magic}");
            }
        }
    }
    
    class Program
    {
        static void Main()
        {
            GameEngine engine = new();

            Character warrior = new Warrior { Name = "Андрей воин" };
            Character mage = new Mage { Name = "Маг Сашка" };

            IWeapon sword = new Sword();
            IWeapon wand = new FoolsWand();
            
            engine.InflictDamage(warrior, mage, sword);
            
            engine.InflictDamage(mage, warrior, wand);
        }
    }
}
