//
//  NetworkManager.swift
//  ShevaStream CRM
//
//  Created by Dmytro Bogatov on 3/9/16.
//  Copyright © 2016 Dmytro Bogatov. All rights reserved.
//

import Foundation
import CoreData

class NetworkManager : NSObject {
	static let sharedInstance = NetworkManager()
	
	var managedObjectContext: NSManagedObjectContext? = nil
	
	func timerAction() {

		print("About to write");
		
		let entity = NSEntityDescription.entityForName("Event", inManagedObjectContext: self.managedObjectContext!)
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
	
	
	private override init() {
		
		super.init()
		
		NSTimer.scheduledTimerWithTimeInterval(5.0, target: self, selector: "timerAction", userInfo: nil, repeats: true)
	
	} //This prevents others from using the default '()' initializer for this class.
}