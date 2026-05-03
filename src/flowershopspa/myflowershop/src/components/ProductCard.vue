<template>
  <div
    class="bg-white rounded-lg shadow-sm overflow-hidden cursor-pointer hover:shadow-md transition-shadow group"
    @click="goToDetail"
  >
    <div class="aspect-square overflow-hidden bg-gray-100">
      <img
        :src="getImageUrl(product.image)"
        :alt="product.name"
        class="w-full h-full object-cover hover:scale-105 transition-transform duration-300"
        @error="onImageError"
      />
    </div>
    
    <div class="p-4">
      <h3 class="text-lg font-semibold text-gray-900 mb-2 line-clamp-2">
        {{ product.name }}
      </h3>
      
      <p class="text-xl font-bold text-pink-600 mb-3">
        {{ formatPrice(product.unitPrice, product.unitCurrency) }}
      </p>

      <!-- Add to Cart button -->
      <button
        @click.stop="handleAddToCart"
        class="w-full flex items-center justify-center gap-2 py-2 px-4 rounded-lg text-sm font-semibold transition-all duration-200"
        :class="added
          ? 'bg-green-500 text-white'
          : 'bg-orange-500 hover:bg-orange-600 text-white'"
      >
        <svg v-if="!added" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13l-1.4 7h12.8M7 13L5.4 5M17 13l1.4 7M9 21a1 1 0 100-2 1 1 0 000 2zm8 0a1 1 0 100-2 1 1 0 000 2z" />
        </svg>
        <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
        </svg>
        {{ added ? 'Added!' : 'Add to Cart' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import type { Flower } from '@/models/flowers/flower'
import { useCartStore } from '@/stores/useCartStore'

const props = defineProps<{
  product: Flower
}>()

const router = useRouter()
const cartStore = useCartStore()
const added = ref(false)

function slugify(name: string): string {
  return name
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
}

function goToDetail() {
  const slug = `${slugify(props.product.name)}-${props.product.id}`
  router.push(`/flower/${slug}`)
}

function handleAddToCart() {
  cartStore.addItem({
    flowerId: props.product.id,
    name: props.product.name,
    price: props.product.unitPrice,
    currency: props.product.unitCurrency,
    image: props.product.image,
    quantity: 1,
  })
  added.value = true
  setTimeout(() => { added.value = false }, 2000)
}

const formatPrice = (price: number, currency?: string) => {
  try {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency || 'USD',
    }).format(price)
  } catch {
    return `${currency ?? ''} ${price}`
  }
}

const getImageUrl = (image: string) => {
  if (!image) return 'https://placehold.co/600x600?text=No+Image'
  if (image.startsWith('http://') || image.startsWith('https://')) return image
  return `/src/assets/images/${image}`
}

function onImageError(event: Event) {
  const img = event.target as HTMLImageElement
  img.src = 'https://placehold.co/600x600?text=No+Image'
}
</script>