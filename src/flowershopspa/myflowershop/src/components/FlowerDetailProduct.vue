<template>
  <div class="bg-white rounded-xl shadow-sm overflow-hidden">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-8 p-6 lg:p-10">
      <FlowerDetailImage :image="flower.image" :name="flower.name" />
      <FlowerDetailInfo :flower="flower" />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { FlowerDetail } from '@/models/flowers/flower'
import FlowerDetailImage from './FlowerDetailImage.vue'
import FlowerDetailInfo from './FlowerDetailInfo.vue'

interface Props {
  flower: FlowerDetail
}

defineProps<Props>()
</script>
