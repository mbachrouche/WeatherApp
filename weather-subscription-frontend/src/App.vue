<template>
  <div class="app">
    <header>
      <h1>Weather Subscription</h1>
    </header>

    <main>
      <div class="columns">
        <section>
          <SubscriptionForm />
        </section>

        <section>
          <div class="card">
            <h2>Check Weather</h2>
            <div class="lookup">
              <input
                v-model="lookupEmail"
                type="email"
                placeholder="Enter your email"
                @keyup.enter="fetchWeather"
              />
              <button @click="fetchWeather" :disabled="lookupLoading">
                {{ lookupLoading ? 'Loading...' : 'Get Weather' }}
              </button>
            </div>
            <p v-if="lookupError" class="message error">{{ lookupError }}</p>
          </div>

          <WeatherCard v-if="weatherData" :weather="weatherData" />
        </section>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SubscriptionForm from './components/SubscriptionForm.vue'
import WeatherCard from './components/WeatherCard.vue'
import { getWeather } from './services/api.js'

const lookupEmail = ref('')
const lookupLoading = ref(false)
const lookupError = ref('')
const weatherData = ref(null)

async function fetchWeather() {
  if (!lookupEmail.value) {
    lookupError.value = 'Please enter your email.'
    return
  }
  lookupError.value = ''
  weatherData.value = null
  lookupLoading.value = true
  try {
    weatherData.value = await getWeather(lookupEmail.value)
  } catch (err) {
    lookupError.value = err.message
  } finally {
    lookupLoading.value = false
  }
}
</script>

<style scoped>
.app {
  min-height: 100vh;
  background: var(--color-bg);
  color: var(--color-text);
}

header {
  padding: 1.5rem 2rem;
  border-bottom: 1px solid var(--color-border);
}

header h1 {
  margin: 0;
  font-size: 1.4rem;
  color: var(--color-heading);
}

main {
  max-width: 900px;
  margin: 0 auto;
  padding: 2rem;
}

.columns {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  align-items: start;
}

@media (max-width: 640px) {
  .columns {
    grid-template-columns: 1fr;
  }
}

.card {
  background: var(--color-card);
  border-radius: 10px;
  padding: 2rem;
  margin-bottom: 1.5rem;
}

h2 {
  margin: 0 0 1.2rem;
  font-size: 1.2rem;
  color: var(--color-heading);
}

.lookup {
  display: flex;
  gap: 0.5rem;
}

input {
  flex: 1;
  padding: 0.6rem 0.8rem;
  border: 1px solid var(--color-border);
  border-radius: 6px;
  background: var(--color-input);
  color: var(--color-text);
  font-size: 0.95rem;
  outline: none;
  transition: border-color 0.2s;
}

input:focus {
  border-color: var(--color-accent);
}

button {
  padding: 0.6rem 1.2rem;
  background: var(--color-accent);
  color: #fff;
  border: none;
  border-radius: 6px;
  font-size: 0.95rem;
  cursor: pointer;
  white-space: nowrap;
  transition: opacity 0.2s;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.message {
  margin: 0.8rem 0 0;
  padding: 0.6rem 0.8rem;
  border-radius: 6px;
  font-size: 0.9rem;
}

.error {
  background: #fde8e8;
  color: #c0392b;
}
</style>
