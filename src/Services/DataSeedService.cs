using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Entities;
using Newtonsoft.Json;
using Shevastream.Extensions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shevastream.Services
{
    public interface IDataSeedService
	{
		/// <summary>
		/// Populates data provider with initial set of records (like enums)
		/// </summary>
		Task SeedDataAsync();

		/// <summary>
		/// A synchronous version of SeedDataAsync.
		/// Does NOT populate data points.
		/// Should be used for testing.
		/// </summary>
		void SeedData();
	}

	public class DataSeedService : IDataSeedService
	{
		private readonly DataContext _context;
		private readonly ICryptoService _crypto;

		private IEnumerable<User> _users;
		private IEnumerable<Product> _products;
		private IEnumerable<Order> _orders;
		private IEnumerable<OrderProduct> _orderProducts;

		public DataSeedService(DataContext context, ICryptoService crypto)
		{
			_context = context;
			_crypto = crypto;

			ReadConfiguration();
		}

		public void SeedData()
		{
			SeedSpecificEntity(_products, _context.Products);
			SeedSpecificEntity(_users, _context.Users);
			SeedSpecificEntity(_orders, _context.Orders);
			SeedSpecificEntity(_orderProducts, _context.OrderProducts);
		}

		public async Task SeedDataAsync()
		{
			await SeedSpecificEntityAsync(_products, _context.Products);
			await SeedSpecificEntityAsync(_users, _context.Users);
			await SeedSpecificEntityAsync(_orders, _context.Orders);
			await SeedSpecificEntityAsync(_orderProducts, _context.OrderProducts);
		}

		private void ReadConfiguration()
		{
			_orderProducts = new List<OrderProduct> {
				new OrderProduct {
					OrderId = 1,
					ProductId = 1
				}
			};

			_orders = new List<Order> {
				new Order {
					Id = 1,
					Address = "100 Institute Road",
					CustomerName = "Dmytro",
					CustomerPhone = "+18577778350",
					CustomerEmail = "dmytro@dbogatov.org",
					ShipmentMethod = "К корпусу Шевченка",
					PaymentMethod = "Наличными",
					Comment = "Wanted to try blue one"
				}
			};

			_users = new List<User> {
				new User {
					Id = 1,
					FullName = "Dmytro Bogatov",
					NickName = "@dmytro",
					PassHash = _crypto.CalculateHash("Doomsday"),
					ImageUrl = "https://shevastream.com/images/team/dmytro.png",
					Position = "Засновник Sheva Stream, програміст",
					Occupation = "Worcester Polytechnic Institute, Computer Science, Class of 2017"
				},
				new User {
					Id = 2,
					FullName = "Polina Guley",
					NickName = "@polly",
					PassHash = _crypto.CalculateHash("cacadoo13"),
					ImageUrl = "https://shevastream.com/images/team/polina.png",
					Position = "Засновниця Sheva Stream",
					Occupation = "КНУ ім. Т. Шевченка"
				},
				new User {
					Id = 3,
					FullName = "Anton Melnikov",
					NickName = "@melnikov",
					PassHash = _crypto.CalculateHash("simplestPossiblePassword123"),
					ImageUrl = "https://shevastream.com/images/team/anton.jpg",
					Position = "Засновник Sheva Stream",
					Occupation = "КНУ ім. Т. Шевченка"
				}
			};

			_products = new List<Product> {
				new Product {
					Id = 1,
					Name = "Блокнот",
					Cost = 185,
					ImageUrls = JsonConvert.SerializeObject(new string[] {
						"/images/products/notepad/item01.jpg",
						"/images/products/notepad/item02.jpg",
						"/images/products/notepad/item03.jpg",
						"/images/products/notepad/item04.jpg",
						"/images/products/notepad/item05.jpg"
					}),
					Description = "Незважаючи на те, що сучасний світ оповитий електронними девайсами з безліччю корисних функцій, ми все одно не припиняємо користуватися блокнотами. Сьогодні блокнот – це не тільки «паперовий друг», котрий завжди під рукою і допоможе навести вам порядок в «інформаційній каші», а й невід’ємний атрибут іміджу кожного з нас.",
					Characteristics = JsonConvert.SerializeObject(new string[] {
						"Обкладинка зроблена з італійської екошкіри",
						"Довжина 210 мм, ширина 145 мм",
						"Стильний паттерн",
						"Міцна кишеня для важливих дрібниць",
						"Закладка-ляссе та зручна гумка",
						"Просторі, чисті, якісні сторінки",
						"Папір Munken Pure, 176 сторінок, 90 г/м<sup>2</sup>"
					}),
					Information = "Збережіть своє натхнення і мрії разом з нашими блокнотами \"ShevaStream\". Мовчазні, практичні, просторі та оригінальні. Посприяють вашій організованості і гарному настрою. Що ще, як не чисті аркуші паперу, потрібно для того, щоб почати діяти - писати історію з нової сторінки. Може на цей раз ви почнете будувати свої глобальні плани та завдання з чимось незвичайним?",
					VideoData = JsonConvert.SerializeObject(new {
						HasVideo = true,
						Url = "https://www.youtube-nocookie.com/embed/jWfdgm3g2GE?rel=0&amp;controls=0&amp;showinfo=0"
					})
				},
				new Product {
					Id = 2,
					Name = "Чохол для телефону",
					Cost = 150,
					ImageUrls = JsonConvert.SerializeObject(new string[] {
						"/images/products/case/item01.jpg",
						"/images/products/case/item02.jpg",
						"/images/products/case/item03.jpg",
						"/images/products/case/item04.jpg"
					}),
					Description = "Давно мріяли про оригінальний чохол для свого девайсу? Пластмасовий чохол для телефону від  Sheva Stream не тільки підкреслить Вашу оригінальність, а й Ваш статус студента найкращого ВНЗ України!",
					Characteristics = JsonConvert.SerializeObject(new string[] {
						"Модель телефону: будь-яка після 2010 року",
						"Матеріал : високоякісний пластик",
						"Глянцеве або матове покриття",
						"Модний університетський дизайн"
					}),
					Information = @"Для замовлення чохлу необхідно зазначити в коментарях до замовлення:
						<ul>
							<li>Модель Вашого телефону</li>
							<li>Тип покриття чохлу ( глянець/мат )</li>
						</ul>",
					VideoData = JsonConvert.SerializeObject(new {
						HasVideo = false,
						Url = ""
					})
				},
				new Product {
					Id = 3,
					Name = "Чохол для телефону VIP",
					Cost = 180,
					ImageUrls = JsonConvert.SerializeObject(new string[] {
						"/images/products/case/item02.jpg",
						"/images/products/case/item01.jpg",
						"/images/products/case/item03.jpg",
						"/images/products/case/item04.jpg"
					}),
					Description = "Мрієте про унікальний чохол для свого девайсу? У Вас є можливість замовити стильний cover для Вашого телефону з індивідуальним дизайном.",
					Characteristics = JsonConvert.SerializeObject(new string[] {
						"Модель телефону: будь-яка після 2010 року",
						"Матеріал : високоякісний пластик / силікон",
						"Глянцеве або матове покриття (для пластику)",
						"Дизайн на Ваш смак <i>(оригінальні написи Вашої фамілії, імені, унікальні картинки, розроблені нашим дизайнером по Вашим побажанням)</i>"
					}),
					Information = @"Для замовлення чохлу необхідно зазначити в коментарях до замовлення:
						<ul>
							<li>Модель Вашого телефону</li>
							<li>Тип покриття чохлу ( глянець/мат )</li>
						</ul>",
					VideoData = JsonConvert.SerializeObject(new {
						HasVideo = false,
						Url = ""
					})
				}
			};
		}

		/// <summary>
		/// A synchronous version of SeedSpecificEntityAsync.
		/// </summary>
		private bool SeedSpecificEntity<T>(IEnumerable<T> values, DbSet<T> dbSets) where T : class
		{
			if (values.Count() != dbSets.Count())
			{
				dbSets.Clear();
				_context.SaveChanges();
				dbSets.AddRange(values);
				_context.SaveChanges();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Replace the existing entities of the T type with the supplied one.
		/// As optimization, it does it only if the number of records in the data provider is different from the number
		/// in the supplied list.
		/// </summary>
		/// <param name="values">List of supplied values to be inserted/replaced</param>
		/// <param name="dbSets">The set of entities of this type in the data provider. 
		/// These are to be replaced by values.</param>
		/// <returns>True if values has been replaced, false if values were in sync and did not require 
		/// replacement.</returns>
		private async Task<bool> SeedSpecificEntityAsync<T>(IEnumerable<T> values, DbSet<T> dbSets, bool ignoreCount = false) where T : class
		{
			if (values.Count() != await dbSets.CountAsync() || ignoreCount)
			{
				if (!ignoreCount)
				{
					dbSets.Clear();
				}
				await _context.SaveChangesAsync();
				await dbSets.AddRangeAsync(values);
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}
	}
}
