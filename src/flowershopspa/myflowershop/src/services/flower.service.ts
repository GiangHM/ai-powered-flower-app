import httpService from '@/services/http.services'
import type { AxiosResponse } from 'axios'
import type {
  AiSearchResponse,
  CartValidationRequest,
  CartValidationResponse,
  Flower,
  FlowerAdminResponse,
  FlowerDetail,
  PagedFlowerResponse
} from '@/models/flowers/flower'

/** Response from the describe-image endpoint. */
export interface DescribeImageResult {
  commonName: string
  flowerType: string,
  notableCharacteristics: string
}

class FlowerService {
  
  // Admin - Flower management
  private readonly flowermanagementEndpoint = 'api/FlowerManagement/Flowers'

  // EShop endpoints
  private readonly searchFlower = 'api/FlowerEshop/Search'
  private readonly allActivatedFlowers = 'api/FlowerEshop/Flowers'
  private readonly validateCartEndpoint = 'api/FlowerEshop/Flowers/validate-cart'

  // AI feature endpoints
  private readonly aisearch = 'api/SemanticSearch/aisearch'
  private readonly describeImageEndpoint = 'api/FlowerManagement/describe-image'

   /**
   * Get all flowers for managing
   * @returns Promise with array of flowers
   */
  public async getFlowerList(): Promise<AxiosResponse<Flower[]>> {
    return await httpService.get<AxiosResponse<Flower[]>>(this.flowermanagementEndpoint)
  }

  public async deleteFlower(id: number): Promise<void> {
    return httpService.delete<void>(`${this.flowermanagementEndpoint}/${id}`)
  }

  public async updateFlowerStatus(id: number, status: boolean): Promise<AxiosResponse<Flower>> {
    console.log("status", status)
    return httpService.put<AxiosResponse<Flower>>(`${this.flowermanagementEndpoint}/${id}/status/${status}`, { status })
  }

 public async createFlower(flower: any): Promise<AxiosResponse<FlowerAdminResponse>> {
    return httpService.post<AxiosResponse<FlowerAdminResponse>>(this.flowermanagementEndpoint, flower)
  }
  /**
   * Get all activated flowers which are ready to sell
   * @returns Promise with array of flowers
   */
  public async getAllActivatedFlowers(): Promise<AxiosResponse<Flower[]>> {
    return await httpService.get<AxiosResponse<Flower[]>>(this.allActivatedFlowers)
  }

  /**
   * Get a paginated list of activated flowers
   * @param page - 1-based page number (default 1)
   * @param pageSize - Number of items per page (default 20)
   * @returns Promise with paginated flower response
   */
  public async getAllActivatedFlowersPaged(
    page: number = 1,
    pageSize: number = 20
  ): Promise<AxiosResponse<PagedFlowerResponse>> {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
    const endpoint = `${this.allActivatedFlowers}?${params.toString()}`
    return await httpService.get<AxiosResponse<PagedFlowerResponse>>(endpoint)
  }

   /**
   * Search flowers by keyword
   * @param searchString - Search keyword
   * @returns Promise with array of flowers matching the keyword
   */
  public async search(searchString: string): Promise<AxiosResponse<Flower[]>> {
    const endpoint = `${this.searchFlower}?keyword=${encodeURIComponent(searchString)}`
    return httpService.get<AxiosResponse<Flower[]>>(endpoint)
  }

  /**
   * Get a single flower by ID for the product detail page
   * @param id - Flower ID
   * @returns Promise with flower detail
   */
  public async getFlowerById(id: number): Promise<AxiosResponse<FlowerDetail>> {
    return httpService.get<AxiosResponse<FlowerDetail>>(`${this.allActivatedFlowers}/${id}`)
  }

  /**
   * AI-powered search for flowers - semantic search
   * @param searchString - Search query for AI
   * @returns Promise with AI search response containing flowers
   */
  public async aiSearch(searchString: string): Promise<AxiosResponse<AiSearchResponse>> {
    const endpoint = `${this.aisearch}/${encodeURIComponent(searchString)}`
    return httpService.get<AxiosResponse<AiSearchResponse>>(endpoint)
  }

  /**
   * Validate cart items against stock and active status
   * @param payload - Cart items and requested quantities
   */
  public async validateCart(
    payload: CartValidationRequest
  ): Promise<AxiosResponse<CartValidationResponse>> {
    return httpService.post<AxiosResponse<CartValidationResponse>, CartValidationRequest>(
      this.validateCartEndpoint,
      payload
    )
  }

  /**
   * Upload a flower image to blob storage
   * @param file - Image file to upload
   * @returns Promise with the uploaded image URL
   */
  public async uploadImage(file: File): Promise<string> {
    const formData = new FormData()
    formData.append('file', file)
    const response = await httpService.post<{ url: string }>(
      `${this.flowermanagementEndpoint}/upload-image`,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    )
    return (response as any).data?.url ?? (response as any).url
  }

  /**
   * Uses GPT-4o vision to identify a flower from an uploaded image file.
   * @param file - Image file of the flower
   * @returns Promise with the identified flower name and short description
   */
  public async describeImage(file: File): Promise<DescribeImageResult> {
    const formData = new FormData()
    formData.append('file', file)
    const response = await httpService.post<DescribeImageResult>(
      this.describeImageEndpoint,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    )
    return (response as any).data ?? response
  }
}

// Export singleton instance
export const flowerService = new FlowerService()