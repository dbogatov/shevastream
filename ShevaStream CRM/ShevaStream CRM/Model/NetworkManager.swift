//
//  NetworkManager.swift
//  ShevaStream CRM
//
//  Created by Dmytro Bogatov on 3/9/16.
//  Copyright © 2016 Dmytro Bogatov. All rights reserved.
//

import Foundation
import CoreData
import Alamofire
import SwiftyJSON

class NetworkManager : NSObject {
	static let sharedInstance = NetworkManager()
	
	var managedObjectContext: NSManagedObjectContext? = nil
	
	func timerAction() {

		print("About to write");
		
		let entity = NSEntityDescription.entityForName("Order", inManagedObjectContext: self.managedObjectContext!)
		let newManagedObject = NSEntityDescription.insertNewObjectForEntityForName(entity!.name!, inManagedObjectContext: self.managedObjectContext!)
		
		// If appropriate, configure the new managed object.
		// Normally you should use accessor methods, but using KVC here avoids the need to add a custom class to the template.
		newManagedObject.setValue(NSDate(), forKey: "timeStamp")
		
		// Save the context.
		do {
			try self.managedObjectContext!.save()
		} catch {
			// Replace this implementation with code to handle the error appropriately.
			// abort() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
			//print("Unresolved error \(error), \(error.userInfo)")
			abort()
		}
		
		print("Written");
	}
	
	func update() {
		print("About to write");
		
		var orders = [OrderModel]();
		
		Alamofire.request(.GET, "http://localhost:5000/api/order")
			.responseJSON { response in
				switch response.result {
				case .Success(let data):
					
					let fetchRequest = NSFetchRequest()
					// Edit the entity name as appropriate.
					let entity = NSEntityDescription.entityForName("Order", inManagedObjectContext: self.managedObjectContext!)
					fetchRequest.entity = entity
					
					// Set the batch size to a suitable number.
					fetchRequest.fetchBatchSize = 20
					
					// Edit the sort key as appropriate.
					let sortDescriptor = NSSortDescriptor(key: "datelastmodified", ascending: false)
					
					fetchRequest.sortDescriptors = [sortDescriptor]
					
					let json = JSON(data)
					for jsonOrder in json.arrayValue {
						let order = OrderModel(
							Id: jsonOrder["Id"].intValue,
							Assignee: jsonOrder["Assignee"].dictionary?["Name"]!.stringValue,
							OrderStatus: jsonOrder["OrderStatus"].dictionaryValue["Description"]!.stringValue,
							Product: jsonOrder["Product"].dictionaryValue["Name"]!.stringValue,
							Quantity: jsonOrder["Quantity"].intValue,
							Cutomer: jsonOrder["Customer"].dictionaryValue["Name"]!.stringValue,
							ShipmentMethod: jsonOrder["ShipmentMethod"].dictionaryValue["Name"]!.stringValue,
							Address: jsonOrder["Address"].stringValue,
							PaymentMethod: jsonOrder["PaymentMethod"].dictionaryValue["Name"]!.stringValue,
							Phone: jsonOrder["Phone"].stringValue,
							Comment: jsonOrder["Comment"].string,
							DateCreated: jsonOrder["DateCreated"].intValue,
							DateLastModified: jsonOrder["DateLastModified"].intValue
						)
						
						orders.append(order)
						
						fetchRequest.predicate = NSPredicate(format: "id == %@", order.Id);
						
						do {
							let fetchedEntities = try self.managedObjectContext!.executeFetchRequest(fetchRequest) as? [Order]
							if fetchedEntities?.count > 0 {
							
								let singleResult = fetchedEntities![0];
								// compare
								
								singleResult.updateObject(order);
							} else {
								
								let entity = NSEntityDescription.entityForName("Order", inManagedObjectContext: self.managedObjectContext!)
								let newManagedObject = NSEntityDescription.insertNewObjectForEntityForName(entity!.name!, inManagedObjectContext: self.managedObjectContext!) as? Order
								
								newManagedObject?.updateObject(order)
							}
							
							// Save the context.
							do {
								try self.managedObjectContext!.save()
							} catch {
								// Replace this implementation with code to handle the error appropriately.
								// abort() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
								//print("Unresolved error \(error), \(error.userInfo)")
								abort()
							}
							
							
						} catch {
							print(error)
						}
					}
					
				case .Failure(let error):
					print("Request failed with error: \(error)")
				}
		}
	}
	
	
	private override init() {
		
		super.init()
		
		//update();
		
		
		//timerAction();
		
		NSTimer.scheduledTimerWithTimeInterval(5.0, target: self, selector: "update", userInfo: nil, repeats: true)
	
	} //This prevents others from using the default '()' initializer for this class.
}