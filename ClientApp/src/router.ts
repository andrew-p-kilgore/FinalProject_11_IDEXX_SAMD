import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
    {
      path: '/avg-request-duration',
      name: 'avg-request-duration',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "counter" */ './views/Counter.vue'),
      },
    {
      path: '/exceptions-graph',
      name: 'exceptions-graph',
      component: () => import(/* webpackChunkName: "testgraph2" */ './views/TestGraph2.vue'),
    },
    {
      path: '/response-types',
      name: 'response-types',
      component: () => import(/* webpackChunkName: "response-types" */'./views/ResponseTypes.vue'),
    },
    {
      path: '/requests-by-endpoint',
      name: 'requests-by-endpoint',
      component: () => import(/* webpackChunkName: "requests-by-endpoint" */ './views/RequestsByEndpoint.vue'),
    },
  ],
});
