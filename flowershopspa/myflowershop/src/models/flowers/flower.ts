// Flower type based on API response
export interface Flower {
  id: number
  name: string
  image:string
  unitPrice: number
  unitCurrency: string
  categoryName: string
}

// For creating/updating flowers (if needed later)
export interface CreateFlowerDto {
  name: string
  unitPrice: number
  unitCurrency: string
  categoryName: string
}

export interface UpdateFlowerDto extends Partial<CreateFlowerDto> {}

export interface AiSearchResponse {
  response?: string
  flowers?: Flower[]
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