import httpService from '@/services/http.services'
import type { AiSearchResponse, Flower, FlowerAdminResponse } from '@/models/flowers/flower'

class FlowerService {
  
  // Admin - Flower management
  private readonly flowermanagementEndpoint = 'api/FlowerManagement/Flowers'

  // EShop endpoints
  private readonly searchFlower = 'api/FlowerEshop/Search'
  private readonly allActivatedFlowers = 'api/FlowerEshop/Flowers'

  // AI feature endpoints
  private readonly aisearch = 'api/SemanticSearch/aisearch'

   /**
   * Get all flowers for managing
   * @returns Promise with array of flowers
   */
  public async getFlowerList(): Promise<Flower[]> {
    return await httpService.get<Flower[]>(this.flowermanagementEndpoint)
  }

  public async deleteFlower(id: number): Promise<void> {
    return httpService.delete<void>(`${this.flowermanagementEndpoint}/${id}`)
  }

  public async updateFlowerStatus(id: number, status: boolean): Promise<Flower> {
    console.log("status", status)
    return httpService.put<Flower>(`${this.flowermanagementEndpoint}/${id}/status/${status}`, { status })
  }

 public async createFlower(flower: any): Promise<FlowerAdminResponse> {
    return httpService.post(this.flowermanagementEndpoint, flower)
  }
  /**
   * Get all activated flowers which are ready to sell
   * @returns Promise with array of flowers
   */
  public async getAllActivatedFlowers(): Promise<Flower[]> {
    return await httpService.get<Flower[]>(this.allActivatedFlowers)
  }

   /**
   * Search flowers by keyword
   * @param searchString - Search keyword
   * @returns Promise with array of flowers matching the keyword
   */
  public async search(searchString: string): Promise<Flower[]> {
    const endpoint = `${this.searchFlower}?keyword=${encodeURIComponent(searchString)}`
    return httpService.get<Flower[]>(endpoint)
  }

  /**
   * AI-powered search for flowers - semantic search
   * @param searchString - Search query for AI
   * @returns Promise with AI search response containing flowers
   */
  public async aiSearch(searchString: string): Promise<AiSearchResponse> {
    const endpoint = `${this.aisearch}/${encodeURIComponent(searchString)}`
    return httpService.get<AiSearchResponse>(endpoint)
  }

  
}

// Export singleton instance
export const flowerService = new FlowerService()