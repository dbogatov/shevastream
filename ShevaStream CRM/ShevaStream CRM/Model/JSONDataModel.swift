//
//  JSONDataModel.swift
//  ShevaStream CRM
//
//  Created by Dmytro Bogatov on 3/9/16.
//  Copyright © 2016 Dmytro Bogatov. All rights reserved.
//

import Foundation
import Gloss

public struct Order {
	
	public let Id: Int
	public let AssigneeId: Int?
	public let OrderStatusId: Int
	public let ProductId: Int
	public let Quantity: Int
	public let CutomerId: Int
	public let ShipmentMethodId: Int
	public let Address: String?
	public let PaymentMethodId: Int
	public let Comment: String?
	public let DateCreated: Int64
	public let DateLastModified: Int64
	
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