export interface UserResponseDto {
  id: number
  name: string
  phone: string
  email: string
  deliveryAddress: string | null
  status: string
  emailVerified: boolean
  creationDate: string
  role: string | null
}

export interface PagedResultDto<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

export interface UpdateUserDto {
  name: string
  phone: string
  deliveryAddress?: string
}

export interface UpdateUserStatusDto {
  status: string
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
  userId: number | null
  deliveryName: string
  deliveryEmail: string
  deliveryPhone: string
  status: string
  totalAmount: number
  orderDate: string
  items: OrderItemResponseDto[]
}
