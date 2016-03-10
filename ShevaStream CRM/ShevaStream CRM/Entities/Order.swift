//
//  Order.swift
//  ShevaStream CRM
//
//  Created by Dmytro Bogatov on 3/10/16.
//  Copyright © 2016 Dmytro Bogatov. All rights reserved.
//

import Foundation
import CoreData


class Order: NSManagedObject {

	// Insert code here to add functionality to your managed object subclass
	func updateObject(model: OrderModel) {
		
		self.id = model.Id
		self.address = model.Address
		self.comment = model.Comment
		self.customer = model.Cutomer
		self.datecreated = model.DateCreated
		self.orderstatus = model.OrderStatus
		self.paymentmethod = model.PaymentMethod
		self.phone = model.Phone
		self.product = model.Product
		self.quantity = model.Quantity
		self.shipmentmethod = model.ShipmentMethod
		self.assignee = model.Assignee
		self.datelastmodified = model.DateLastModified
		
	}
	
}
