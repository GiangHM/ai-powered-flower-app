export interface OrderItemDto {
  flowerId: number
  quantity: number
}

export interface CreateOrderDto {
  customerId: number | null
  deliveryName?: string
  deliveryEmail?: string
  deliveryPhone?: string
  items: OrderItemDto[]
}

export interface OrderItemResponseDto {
  id: number
  flowerId: number
  flowerName: string
  quantity: number
  unitPrice: number
}

export interface OrderResponseDto {
  id: number
  customerId: number | null
  deliveryName: string
  deliveryEmail: string
  deliveryPhone: string
  status: string
  totalAmount: number
  orderDate: string
  items: OrderItemResponseDto[]
}

export interface CheckoutDeliveryInfo {
  fullName: string
  email: string
  phoneNumber: string
  deliveryAddress: string
  orderId: number
}

export interface LastOrderSession {
  orderId: number
  fullName: string
  email: string
  deliveryAddress: string
  items: LastOrderItem[]
  totalAmount: number
  status: string
  orderDate: string
}

export interface LastOrderItem {
  flowerId: number
  name: string
  price: number
  currency: string
  image: string
  quantity: number
}

export interface UpdateOrderStatusDto {
  status: string
}
