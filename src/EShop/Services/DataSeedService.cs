using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using Newtonsoft.Json;

namespace EShop.Services
{
    public interface IDataSeedService
	{
		void SeedData();
	}

	public class DataSeedService : IDataSeedService
	{
		private readonly DataContext _context;
		private readonly ICryptoService _crypto;

		public DataSeedService(DataContext context, ICryptoService crypto)
		{
			_context = context;
			_crypto = crypto;
		}

		public void SeedData()
		{
			var shipmentMethods = new List<ShipmentMethod> {
				new ShipmentMethod { Id = 1, Name = "К корпусу Шевченка", Cost = 0, CostTBD = false },
				new ShipmentMethod { Id = 2, Name = "По Киеву", Cost = 30, CostTBD = false },
				new ShipmentMethod { Id = 3, Name = "Новой Почтой", Cost = 0, CostTBD = true }
			};

			var paymentMethods = new List<PaymentMethod> {
				new PaymentMethod { Id = 1, Name = "Наличными" },
				new PaymentMethod { Id = 2, Name = "Через систему \"Приват 24\"" }
			};

			var products = new List<Product> {
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

			var users = new List<User> {
				new User { Id = 1, FullName = "Dmytro Bogatov", NickName = "@dmytro", PassHash = _crypto.CalculateHash("Doomsday"), ImageUrl = "https://shevastream.com/images/team/Dmytro.png" },
				new User { Id = 2, FullName = "Polina Guley", NickName = "@polly", PassHash = _crypto.CalculateHash("cacadoo13"), ImageUrl = "https://shevastream.com/images/team/Polina.png" },
				new User { Id = 3, FullName = "Anton Melnikov", NickName = "@melnikov", PassHash = _crypto.CalculateHash("simplestPossiblePassword123"), ImageUrl = "https://shevastream.com/images/team/Anton.jpg" },
				new User { Id = 4, FullName = "Taras Shevchenko", NickName = "@none", PassHash = _crypto.CalculateHash("cabooom45"), ImageUrl = "#" }
			};

			var orderStatuses = new List<OrderStatus> {
				new OrderStatus { Id = 1, Description = "Received" },
				new OrderStatus { Id = 2, Description = "In progress" },
				new OrderStatus { Id = 3, Description = "Waiting for shipment" },
				new OrderStatus { Id = 4, Description = "Done" }
			};

			if (_context.ShipmentMethods.Count() != shipmentMethods.Count())
			{
				_context.ShipmentMethods.Clear();
				_context.ShipmentMethods.AddRange(shipmentMethods);
			}

			if (_context.PaymentMethods.Count() != paymentMethods.Count())
			{
				_context.PaymentMethods.Clear();
				_context.PaymentMethods.AddRange(paymentMethods);
			}

			if (_context.Products.Count() != products.Count())
			{
				_context.Products.Clear();
				_context.SaveChanges();
				_context.Products.AddRange(products);
			}

			if (_context.Users.Count() != users.Count())
			{
				_context.Users.Clear();
				_context.Users.AddRange(users);
			}

			if (_context.OrderStatuses.Count() != orderStatuses.Count())
			{
				_context.OrderStatuses.Clear();
				_context.OrderStatuses.AddRange(orderStatuses);
			}

			if (!_context.Customers.Any())
			{
				_context.AddRange(
					new Customer
					{
						Name = "Dmytro",
						Phone = "(050) 866-22-22",
						Email = "dbogatov@wpi.edu"
					}
				);
			}

			_context.SaveChanges();

			if (!_context.Orders.Any())
			{
				_context.AddRange(
					new Order
					{
						Quantity = 2,
						Address = "100 Institute Road",
						AssigneeId = _context.Users.FirstOrDefault().Id,
						OrderStatusId = _context.OrderStatuses.FirstOrDefault().Id,
						ProductId = _context.Products.FirstOrDefault().Id,
						CustomerId = _context.Customers.FirstOrDefault().Id,
						ShipmentMethodId = _context.ShipmentMethods.FirstOrDefault().Id,
						PaymentMethodId = _context.PaymentMethods.FirstOrDefault().Id,
						Comment = "Wanted to try blue one",
						AssigneeComment = "Got it",
						DateCreated = DateTime.Now.Ticks,
						DateLastModified = DateTime.Now.Ticks
					}
				);
			}

			_context.SaveChanges();

		}
	}
}
