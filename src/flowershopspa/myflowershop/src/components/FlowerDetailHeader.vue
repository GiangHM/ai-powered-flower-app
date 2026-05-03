<template>
  <header class="bg-white shadow-sm">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center gap-4">
      <button
        @click="router.back()"
        class="flex items-center gap-2 text-gray-600 hover:text-pink-600 transition-colors"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
        </svg>
        Back
      </button>
      <h1 class="text-2xl font-bold text-gray-900 flex-1">Flower Shop</h1>
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
  </header>

  <ShoppingCart v-model="cartOpen" />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ShoppingCart from '@/components/ShoppingCart.vue'
import { useCartStore } from '@/stores/useCartStore'

const router = useRouter()
const cartStore = useCartStore()
const cartOpen = ref(false)
</script>
