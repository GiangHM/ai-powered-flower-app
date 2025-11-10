<template>
  <div class="bg-white rounded-lg shadow-sm overflow-hidden">
    <div class="aspect-square overflow-hidden bg-gray-100">
      <img
        :src="getImageUrl(product.image)"
        :alt="product.name"
        class="w-full h-full object-cover"
      />
    </div>
    
    <div class="p-4">
      <h3 class="text-lg font-semibold text-gray-900 mb-2">
        {{ product.name }}
      </h3>
      
      <p class="text-xl font-bold text-pink-600">
        {{ formatPrice(product.unitPrice) }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

defineProps({
  product: {
    type: Object,
    required: true
  }
})

const formatPrice = (price: any) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(price)
}

const getImageUrl = (image: any) => {
  return `/src/assets/images/${image}`
}
</script>