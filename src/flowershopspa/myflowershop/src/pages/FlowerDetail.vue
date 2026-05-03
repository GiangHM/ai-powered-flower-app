<template>
  <div class="min-h-screen bg-gray-50">
    <FlowerDetailHeader />

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="isLoading" class="flex justify-center items-center py-24">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-pink-600"></div>
        <p class="ml-4 text-gray-600">Loading flower details...</p>
      </div>

      <div v-else-if="error" class="text-center py-24">
        <svg class="mx-auto w-16 h-16 text-red-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <h3 class="text-lg font-medium text-gray-900 mb-2">Flower Not Found</h3>
        <p class="text-gray-600 mb-6">{{ error }}</p>
        <button
          @click="router.push('/')"
          class="bg-pink-600 text-white px-6 py-2 rounded-lg hover:bg-pink-700 transition-colors"
        >
          Back to Shop
        </button>
      </div>

      <FlowerDetailProduct v-else-if="flower" :flower="flower" />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { FlowerDetail } from '@/models/flowers/flower'
import { flowerService } from '@/services/flower.service'
import FlowerDetailHeader from '@/components/FlowerDetailHeader.vue'
import FlowerDetailProduct from '@/components/FlowerDetailProduct.vue'

const route = useRoute()
const router = useRouter()

const flower = ref<FlowerDetail | null>(null)
const isLoading = ref(true)
const error = ref<string | null>(null)

/**
 * Extract numeric ID from SEO-friendly slug: "rose-bouquet-42" → 42
 */
function extractIdFromSlug(slug: string): number | null {
  const parts = slug.split('-')
  const last = parts[parts.length - 1]
  if (!last) return null
  const id = parseInt(last, 10)
  return isNaN(id) ? null : id
}

async function fetchFlower() {
  isLoading.value = true
  error.value = null

  const slug = route.params.slug as string
  const id = extractIdFromSlug(slug)

  if (!id) {
    error.value = 'Invalid flower URL.'
    isLoading.value = false
    return
  }

  try {
    const response = await flowerService.getFlowerById(id)
    flower.value = response.data
  } catch (err: any) {
    error.value = err?.response?.status === 404
      ? 'This flower could not be found.'
      : 'Failed to load flower details. Please try again.'
  } finally {
    isLoading.value = false
  }
}

onMounted(fetchFlower)
</script>
