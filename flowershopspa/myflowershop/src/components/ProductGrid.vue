<template>
  <div v-if="products.length > 0" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
    <ProductCard
      v-for="product in products"
      :key="product.id"
      :product="product"
    />
  </div>
  
  <div v-else class="text-center py-16">
    <svg class="mx-auto h-16 w-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
    </svg>
    <p class="mt-4 text-lg text-gray-600">No products found</p>
    <p class="text-gray-500">Try adjusting your search terms</p>
  </div>
</template>

<script setup lang="ts">
import ProductCard from './ProductCard.vue'

defineProps({
  products: {
    type: Object,
    required: true
  }
})
</script>