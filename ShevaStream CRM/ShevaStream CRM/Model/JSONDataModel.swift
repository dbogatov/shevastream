//
//  JSONDataModel.swift
//  ShevaStream CRM
//
//  Created by Dmytro Bogatov on 3/9/16.
//  Copyright © 2016 Dmytro Bogatov. All rights reserved.
//

import Foundation
import Gloss

public struct OrderModel {
	
	public let Id: NSNumber
	public let Assignee: String?
	public let OrderStatus: String
	public let Product: String
	public let Quantity: NSNumber
	public let Cutomer: String
	public let ShipmentMethod: String
	public let Address: String?
	public let PaymentMethod: String
	public let Phone: String
	public let Comment: String?
	public let DateCreated: NSNumber
	public let DateLastModified: NSNumber
	
	/*
	public init?(json: JSON) {
		guard let
			Id: Int = "Id" <~~ json,
			OrderStatusId: Int = "OrderStatusId" <~~ json,
			ProductId: Int = "ProductId" <~~ json,
			Quantity: Int = "Quantity" <~~ json,
			CutomerId: Int = "CutomerId" <~~ json,
			ShipmentMethodId: Int = "ShipmentMethodId" <~~ json,
			PaymentMethodId: Int = "PaymentMethodId" <~~ json,
			DateCreated: Int64 = "DateCreated" <~~ json,
			DateLastModified: Int64 = "DateLastModified" <~~ json
			else { return nil }
		
		self.Id = Id
		self.OrderStatusId = OrderStatusId
		self.ProductId = ProductId
		self.Quantity = Quantity
		self.CutomerId = CutomerId
		self.ShipmentMethodId = ShipmentMethodId
		self.PaymentMethodId = PaymentMethodId
		self.DateCreated = DateCreated
		self.DateLastModified = DateLastModified
		
		self.AssigneeId = "AssigneeId" <~~ json
		self.Address = "Address" <~~ json
		self.Comment = "Comment" <~~ json
	}*/
}