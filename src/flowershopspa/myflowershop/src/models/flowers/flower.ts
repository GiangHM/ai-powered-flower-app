// Flower type based on API response
export interface Flower {
  id: number
  name: string
  image:string
  unitPrice: number
  unitCurrency: string
  categoryName: string
}

export interface FlowerDetail extends Flower {
  description?: string
  stockQuantity: number
}

// For creating/updating flowers (if needed later)
export interface CreateFlowerDto {
  name: string
  unitPrice: number
  unitCurrency: string
  categoryName: string
}

export interface UpdateFlowerDto extends Partial<CreateFlowerDto> {}

export interface PagedFlowerResponse {
  items: Flower[]
  totalCount: number
  page: number
  pageSize: number
}

export interface AiSearchResponse {
  response?: string
  flowers?: Flower[]
}

export interface CartValidationItem {
  flowerId: number
  quantity: number
}

export interface CartValidationRequest {
  items: CartValidationItem[]
}

export type CartValidationStatus = 'available' | 'out_of_stock' | 'inactive' | 'not_found'

export interface CartItemValidationResult {
  flowerId: number
  requestedQuantity: number
  status: CartValidationStatus
}

export interface CartValidationResponse {
  results: CartItemValidationResult[]
}

export interface FlowerAdminResponse {
  id: number
  name: string
  unitPrice: number
  unitCurrency: string
  categoryName: string
  status: boolean
}
export interface CreateFlowerFormDto {
  name: string
  description?: string
  imageUrl: string
  categoryId: number
  unitPrice: number
  unitCurrency: string
}