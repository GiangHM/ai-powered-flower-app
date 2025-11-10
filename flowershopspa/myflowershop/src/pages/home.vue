<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <h1 class="text-3xl font-bold text-gray-900">Flower Shop</h1>
        <p class="text-gray-600 mt-1">Beautiful flowers for every occasion</p>
        <SearchBar
          v-model="searchQuery"
          v-model:use-semantic-search="useSemanticSearch"
          :results-count="filteredProducts.length"
          @search="handleSearch"
        />
      </div>

    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="isLoading" class="text-center py-12">
        <div class="inline-block">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        </div>
        <p class="mt-4 text-gray-600">Searching flowers...</p>
      </div>

      <div v-else-if="searchError" class="text-center py-12">
        <svg class="mx-auto w-16 h-16 text-red-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <h3 class="text-lg font-medium text-gray-900 mb-2">Search Error</h3>
        <p class="text-gray-600">{{ searchError }}</p>
      </div>
      <div v-else>
        <!-- Display AI search response message above products -->
        <div v-if="aiSearchMessage" class="mb-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
          <p class="text-blue-900 text-lg">{{ aiSearchMessage }}</p>
        </div>
        <ProductGrid :products="filteredProducts" />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import SearchBar from '@/components/SearchBar.vue';
import ProductGrid from '@/components/ProductGrid.vue';
import type { Flower } from '@/models/flowers/flower';
import { flowerService } from '@/services/flower.service';

const searchQuery = ref('')
const useSemanticSearch = ref(false)
const isLoading = ref(false)
const searchError = ref<string | null>(null)
const aiSearchMessage = ref<string | null>(null)

const flowers = ref<Flower[]>([])
const loading = ref<boolean>(false)
const error = ref<string | null>(null)
const searchResults = ref<Flower[]>([])
const hasSearched = ref(false)

const fetchFlowers = async (): Promise<void> => {
  loading.value = true
  error.value = null
  
  try {
    const response = await flowerService.getAllActivatedFlowers()
    flowers.value = response.data
    //console.log('Fetched flowers:', response)
  } catch (err: any) {
    error.value = err.message || 'Failed to load flowers'
    console.error('[v0] Error fetching flowers:', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchFlowers()
})

const filteredProducts = computed(() => {
  return hasSearched.value ? searchResults.value : flowers.value

})

const handleSearch = async (searchData: { query: string; useSemanticSearch: boolean }) => {
  if (!searchData.query.trim()) return

  isLoading.value = true
  searchError.value = null
  hasSearched.value = true
  aiSearchMessage.value = null

  try {
    if (searchData.useSemanticSearch) {
      const aisearchRes = await flowerService.aiSearch(searchData.query)
      //console.log('[AI search response:', aisearchRes)
      
      if (aisearchRes.data.response) {
        aiSearchMessage.value = aisearchRes.data.response
      }
      
      if (aisearchRes.data.flowers && Array.isArray(aisearchRes.data.flowers)) {
        searchResults.value = aisearchRes.data.flowers
      } else {
        searchResults.value = []
      }
    } else {
      aiSearchMessage.value = null
      const results = await flowerService.search(searchData.query)
      //console.log(' Keyword search results:', results)
      searchResults.value = results.data
    }
  } catch (error: any) {
    searchError.value = error.message || 'Search failed. Please try again.'
    console.error('Search error:', error)
    searchResults.value = []
  } finally {
    isLoading.value = false
  }
}
</script>