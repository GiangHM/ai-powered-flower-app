import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { CartItem } from '@/models/cart'

function loadCartFromStorage(): CartItem[] {
  try {
    const stored = localStorage.getItem('cart')
    if (!stored) return []
    const parsed = JSON.parse(stored)
    if (!Array.isArray(parsed)) return []
    return parsed as CartItem[]
  } catch {
    return []
  }
}

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>(loadCartFromStorage())

  const totalCount = computed(() =>
    items.value.reduce((sum, i) => sum + i.quantity, 0)
  )

  const subtotal = computed(() =>
    items.value.reduce((sum, i) => sum + i.price * i.quantity, 0)
  )

  function addItem(item: CartItem) {
    const existing = items.value.find(i => i.flowerId === item.flowerId)
    if (existing) {
      existing.quantity += item.quantity
    } else {
      items.value.push({ ...item })
    }
    persist()
  }

  function removeItem(flowerId: number) {
    items.value = items.value.filter(i => i.flowerId !== flowerId)
    persist()
  }

  function updateQuantity(flowerId: number, quantity: number) {
    if (quantity <= 0) {
      removeItem(flowerId)
      return
    }
    const existing = items.value.find(i => i.flowerId === flowerId)
    if (existing) {
      existing.quantity = quantity
      persist()
    }
  }

  function clearCart() {
    items.value = []
    persist()
  }

  function persist() {
    localStorage.setItem('cart', JSON.stringify(items.value))
  }

  return { items, totalCount, subtotal, addItem, removeItem, updateQuantity, clearCart }
})
