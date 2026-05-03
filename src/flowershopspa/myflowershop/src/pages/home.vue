<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
          </div>
          <!-- Cart icon -->
          <button
            @click="cartOpen = true"
            class="relative p-2 text-gray-600 hover:text-pink-600 transition-colors"
            aria-label="Open cart"
          >
            <svg class="w-7 h-7" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            <span
              v-if="cartStore.totalCount > 0"
              class="absolute -top-1 -right-1 min-w-[1.25rem] h-5 flex items-center justify-center rounded-full bg-orange-500 text-white text-xs font-bold px-1"
            >{{ cartStore.totalCount }}</span>
          </button>
        </div>
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
        <!-- Pagination (only shown when not in search mode) -->
        <Pagination
          v-if="!hasSearched"
          :current-page="currentPage"
          :page-size="pageSize"
          :total-count="totalCount"
          @update:page="goToPage"
        />
      </div>
    </main>

    <ShoppingCart v-model="cartOpen" />
    <ChatWidget />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import SearchBar from '@/components/SearchBar.vue';
import ProductGrid from '@/components/ProductGrid.vue';
import Pagination from '@/components/Pagination.vue';
import ShoppingCart from '@/components/ShoppingCart.vue';
import ChatWidget from '@/components/ChatWidget.vue';
import type { Flower } from '@/models/flowers/flower';
import { flowerService } from '@/services/flower.service';
import { useCartStore } from '@/stores/useCartStore';

const cartStore = useCartStore()
const cartOpen = ref(false)

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

// Pagination state
const currentPage = ref(1)
const pageSize = ref(20)
const totalCount = ref(0)

const fetchFlowers = async (page: number = 1): Promise<void> => {
  loading.value = true
  error.value = null

  try {
    const response = await flowerService.getAllActivatedFlowersPaged(page, pageSize.value)
    const data = response.data
    flowers.value = data.items
    totalCount.value = data.totalCount
    currentPage.value = data.page
  } catch (err: any) {
    error.value = err.message || 'Failed to load flowers'
    console.error('[v0] Error fetching flowers:', err)
  } finally {
    loading.value = false
  }
}

const goToPage = (page: number): void => {
  const maxPage = Math.ceil(totalCount.value / pageSize.value) || 1
  if (page < 1 || page > maxPage) return
  fetchFlowers(page)
  window.scrollTo({ top: 0, behavior: 'smooth' })
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