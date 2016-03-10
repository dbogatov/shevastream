//
//  Order+CoreDataProperties.swift
//  ShevaStream CRM
//
//  Created by Dmytro Bogatov on 3/10/16.
//  Copyright © 2016 Dmytro Bogatov. All rights reserved.
//
//  Choose "Create NSManagedObject Subclass…" from the Core Data editor menu
//  to delete and recreate this implementation file for your updated model.
//

import Foundation
import CoreData

extension Order {

    @NSManaged var address: String?
    @NSManaged var comment: String?
    @NSManaged var customer: String?
	@NSManaged var orderstatus: String?
    @NSManaged var datecreated: NSNumber?
    @NSManaged var id: NSNumber?
    @NSManaged var paymentmethod: String?
    @NSManaged var phone: String?
    @NSManaged var product: String?
    @NSManaged var quantity: NSNumber?
    @NSManaged var shipmentmethod: String?
    @NSManaged var assignee: String?
    @NSManaged var datelastmodified: NSNumber?

}
